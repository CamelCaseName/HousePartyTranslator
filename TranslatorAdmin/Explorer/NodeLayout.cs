using Silk.NET.Core.Contexts;
using Silk.NET.OpenCL;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Versioning;
using Translator.Core;

namespace Translator.Explorer
{
	internal static class StoryExplorerConstants
	{
		public const float Attraction = 0.2f;//Attraction accelleration multiplier, between 0 and 1
		public const float Repulsion = 100.0f;//Repulsion accelleration multiplier, between 0 and much
		public const float Gravity = 0.0003f;
		public static float IdealLength = 120; //spring IdealLength in units aka thedistance an edge should be long
		public static int ColoringDepth = 15;
		public static int Nodesize = 16;
	}

	[SupportedOSPlatform("windows")]
	internal unsafe sealed class NodeLayout
	{
		private List<Vector2> NodeForces = new();
		private DateTime StartTime = DateTime.MinValue;
		private DateTime FrameStartTime = DateTime.MinValue;
		private DateTime FrameEndTime = DateTime.MinValue;
		public bool RunOverride = false;
		private CancellationTokenSource cancellationToken = new();
		private readonly CancellationToken outsideToken;
		private readonly NodeProvider provider;
		private readonly CL _cl;
		private readonly INativeContext _context;
		private readonly Dictionary<nint, (nint deviceId, string platformName)> Platforms = new();
		nint preselectedPlatform = 0;
		private nint _selectedDevice;
		public int FrameCount { get; private set; }
		public bool Finished => FrameCount > Nodes.Count * Nodes.Count && !RunOverride && Started;
		public bool Started { get; private set; } = false;
		public List<Node> Nodes
		{
			get { return provider.Nodes; }
		}
		private List<Node> Internal
		{
			get { return provider.OtherNodes; }
		}

		public NodeLayout(NodeProvider provider, CancellationToken cancellation)
		{
			outsideToken = cancellation;
			this.provider = provider;

			//opencl stuff
			//get api and context
			_cl = CL.GetApi();
			_context = _cl.Context;

			//get platform
			FindOpenCLPlatforms();
			//just do the old way if we have no opencl
			if (Platforms.Count == 0) return;


			//todo let user choose device, also allow combination as it should work regardless, it is pretty good in parallelization
			
			//build program
			string kernelText = File.ReadAllText(".\\opencl\\layout.cl");
			var program = _cl.CreateProgramWithSource(_context.GetProcAddress("", 0), 1, new[] { kernelText }, null, out int err);
			if (err != 0) Debug.WriteLine($"err is {err}");
			err = _cl.BuildProgram(program, 1, new[] { _selectedDevice }, Array.Empty<byte>(), null, null);
			//get build status as it is not in the error
			BuildStatus status;
			//todo see docs for datatypes returned
			//status = _cl.GetProgramBuildInfo(program, _selectedDevice, ProgramBuildInfo.BuildStatus, out err);
			//if (status != BuildStatus.Success)
			//{
			//	var log = _cl.GetProgramBuildInfo(program, _dev_selectedDeviceice, ProgramBuildInfo.BuildLog, out err);
			//}
		}

		private void FindOpenCLPlatforms()
		{
			_ = _cl.GetPlatformIDs(0, null, out uint platformCount);
			nint[] _platforms = new nint[platformCount];
			Debug.WriteLine($"Found {platformCount} opencl platforms");
			_ = _cl.GetPlatformIDs(platformCount, _platforms, Array.Empty<uint>());

			//get devices
			//platform id is key
			nuint maxValue = 0;
			for (int i = 0; i < platformCount; i++)
			{
				//get length/size of name
				_ = _cl.GetPlatformInfo(_platforms[i], PlatformInfo.Name, 0, null, out nuint NameLength);
				sbyte[] platformName = new sbyte[NameLength + 1];
				//get platform name
				string platformNameString = string.Empty;
				fixed (sbyte* s = platformName)
				{
					_ = _cl.GetPlatformInfo(_platforms[i], PlatformInfo.Name, NameLength, s, out _);
					platformNameString = new(s);
					Debug.WriteLine($"    Platform: {platformNameString}");
				}
				//get device count
				_ = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, 0, null, out uint deviceCount);
				Debug.WriteLine($"        this platform has: {deviceCount} opencl devices");
				var _devices = new nint[deviceCount];
				//get devices
				_ = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, deviceCount, _devices, Array.Empty<uint>());
				//find device with most power, opencl compute units
				for (int k = 0; k < deviceCount; k++)
				{
					nuint maxComputeUnits = 0;
					_ = _cl.GetDeviceInfo(_devices[k], DeviceInfo.MaxWorkGroupSize, (nuint)sizeof(nuint), &maxComputeUnits, out _);

					//get length/size of name
					_ = _cl.GetDeviceInfo(_devices[k], DeviceInfo.Name, 0, null, out nuint DeviceNameLength);
					sbyte[] deviceName = new sbyte[DeviceNameLength + 1];
					//get device name
					fixed (sbyte* s = deviceName)
					{
						_ = _cl.GetDeviceInfo(_devices[k], DeviceInfo.Name, NameLength, &s, out _);
						string deviceNamestring = new(s);
						Debug.WriteLine($"            {deviceNamestring} with {maxComputeUnits} workgroupsize");
						if (maxComputeUnits > maxValue)
						{
							Debug.WriteLine("        >>>>chose the one above");
							maxValue = maxComputeUnits;
							preselectedPlatform = _platforms[i];
							Platforms.Add(_platforms[i], (_devices[k], platformNameString));
							_selectedDevice = _devices[k];
						}
					}
				}
			}
			//now we should have the best device
			Debug.WriteLine($"suggested device {Platforms[preselectedPlatform].platformName}");
		}

		public void Start()
		{
			if (!Started)
			{
				cancellationToken = new();
				outsideToken.Register(() => cancellationToken.Cancel());
				StartTime = DateTime.Now;
				LogManager.Log($"\tnode layout started for {Nodes.Count} nodes");
				Started = true;
				_ = Task.Run(() => CalculateForceDirectedLayout(cancellationToken.Token), cancellationToken.Token);
			}
		}

		public void Stop()
		{
			DateTime end = DateTime.Now;
			LogManager.Log($"\tnode layout ended, rendered for {(end - StartTime).TotalSeconds:F2} seconds and rendered {FrameCount} frames -> {FrameCount / (end - StartTime).TotalSeconds:F2} fps");
			cancellationToken.Cancel();
			Started = false;
			FrameCount = 0;
		}

		public void CalculateForceDirectedLayout(CancellationToken token)
		{
			//save all forces here
			NodeForces = new(Nodes.Count);
			Nodes.ForEach(n => NodeForces.Add(new Vector2()));

			while (!token.IsCancellationRequested && !Finished)
			{
				FrameStartTime = FrameEndTime;
				//sync nodes, initally in b

				//calculate
				CalculatePositions();
				CalculatePositions();

				//we got to wait before we change nodes, so like a reverse lock?
				while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
				//switch to other list once done
				provider.UsingListA = !provider.UsingListA;
				++FrameCount;

				//approx 40fps max as more is uneccesary and feels weird (25ms gives ~50fps, 30 gives about 45fps)
				FrameEndTime = DateTime.Now;
				if ((FrameEndTime - FrameStartTime).TotalMilliseconds < 30) Thread.Sleep((int)(30 - (FrameEndTime - FrameStartTime).TotalMilliseconds));

				App.MainForm?.Explorer?.Invalidate();
			}
			Stop();
		}

		//todo move to gpu with opencl
		private void CalculatePositions()
		{
			for (int i = 0; i < NodeForces.Count; i++)
			{
				NodeForces[i] = Vector2.Zero;
			}

			for (int first = 0; first < Internal.Count; first++)
			{

				//Gravity to center
				float radius = MathF.Sqrt(Internal.Count) + StoryExplorerConstants.IdealLength * 2;
				Vector2 pos = new(Internal[first].Position.X, Internal[first].Position.Y);

				//fix any issues before we divide by pos IdealLength
				if (pos.Length() == 0)
				{
					pos.X = pos.X == 0 ? float.MinValue : pos.X;
					pos.Y = pos.Y == 0 ? float.MinValue : pos.Y;
				}

				//can IdealLength ever be absolutely 0?
				//gravity calculation
				NodeForces[first] -= (pos / pos.Length()) * MathF.Pow(MathF.Abs(pos.Length() - radius), 1.5f) * MathF.Sign(pos.Length() - radius) * StoryExplorerConstants.Gravity;

				for (int second = first + 1; second < Internal.Count; second++)
				{
					if (Internal[first].Position != Internal[second].Position)
					{
						Vector2 edge = new(
								Internal[first].Position.X - Internal[second].Position.X,
								Internal[first].Position.Y - Internal[second].Position.Y
								);
						if (Internal[first].ChildNodes.Contains(Internal[second]) || Internal[first].ParentNodes.Contains(Internal[second]))
						{
							//Attraction/spring accelleration on edge
							Vector2 attractionVec = (edge / edge.Length()) * StoryExplorerConstants.Attraction * (edge.Length() - StoryExplorerConstants.IdealLength);

							NodeForces[first] -= attractionVec;
							NodeForces[second] += attractionVec;
						}
						else
						{
							//Repulsion
							NodeForces[first] += (edge / edge.LengthSquared()) * StoryExplorerConstants.Repulsion;
							NodeForces[second] -= (edge / edge.LengthSquared()) * StoryExplorerConstants.Repulsion;
						}
					}
					else
					{
						//move away
						Internal[first].Position.X += 10f;
					}
				}
			}

			//apply accelleration to nodes
			for (int i = 0; i < Internal.Count; i++)
			{
				if (!Internal[i].IsPositionLocked)
				{
					Internal[i].Position.X += NodeForces[i].X / Internal[i].Mass;
					Internal[i].Position.Y += NodeForces[i].Y / Internal[i].Mass;
				}
			}
		}
	}
}
