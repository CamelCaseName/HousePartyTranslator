using Silk.NET.OpenCL;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Translator.Core;

namespace Translator.Explorer.OpenCL;
[SupportedOSPlatform("windows")]
internal sealed unsafe class OpenCLManager
{
	private nint preselectedPlatform = 0;//pointer to preselected platform
	private nint SelectedPlatform = 0;
	private readonly CL _cl;
	private readonly Dictionary<nint, (nint deviceId, string platformName)> Platforms = new();//key is pointer to platform
	private string DeviceName = string.Empty;
	private readonly string KernelName = "layout_kernel";
	private nint _context;//pointer to context
	private nint _commandQueue;//pointer to commandqueue for our selected device
	private nint _device;
	private nint _program;
	private nint _kernel;
	public bool OpenCLDevicePresent = false;
	private bool Failed = false;
	private readonly Form parent;
	private readonly NodeProvider Provider;

	public OpenCLManager(Form parent, NodeProvider provider)
	{
		Provider = provider;
		this.parent = parent;
		//get api and context
		_cl = CL.GetApi();
	}

	public void SetUpOpenCl()
	{
		SetUpOpenCLImpl();
		if (Failed)
		{
			if (_commandQueue != nint.Zero)
				_cl.ReleaseCommandQueue(_commandQueue);
			if (_kernel != nint.Zero)
				_cl.ReleaseKernel(_kernel);
			if (_program != nint.Zero)
				_cl.ReleaseProgram(_program);
			if (_device != nint.Zero)
				_cl.ReleaseDevice(_device);
			if (_context != nint.Zero)
				_cl.ReleaseContext(_context);
		}
	}

	private void SetUpOpenCLImpl()
	{
		//get platform
		FindOpenCLPlatforms();
		//just do the old way if we have no opencl
		if (Platforms.Count == 0) return;
		OpenCLDevicePresent = true;

		_device = SelectDevice();
		if (_device == nint.Zero) return;

		DeviceName = Platforms[SelectedPlatform].platformName;

		//create contet, program and build it on the selected device
		CreateProgram();
		if (Failed) return;
		//success, we can go on creating the kernel
		_kernel = _cl.CreateKernel(_program, KernelName, out int err);
		if (Failed |= err != 0) return;

		//create and fill buffers on cpu side
		var nodePositionBuffer = Provider.GetNodePositionBuffer();
		var returnedNodePositionBuffer = Provider.GetNodeNewPositionBuffer();
		(int[] nodeParents, int[] nodeParentsOffset, int[] nodeParentsCount) = Provider.GetNodeParentsBuffer();
		(int[] nodeChilds, int[] nodeChildsOffset, int[] nodeChildsCount) = Provider.GetNodeChildsBuffer();

		//create buffers on gpu
		var node_pos = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodePositionBuffer.Length * sizeof(float) * 4), nodePositionBuffer.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var new_node_pos = _cl.CreateBuffer(_context, MemFlags.WriteOnly, (nuint)(returnedNodePositionBuffer.Length * sizeof(float) * 4), returnedNodePositionBuffer.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var node_parents = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodeParents.Length * sizeof(int)), nodeParents.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var node_parent_offset = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodeParentsOffset.Length * sizeof(int)), nodeParentsOffset.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var node_parent_count = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodeParentsCount.Length * sizeof(int)), nodeParentsCount.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var node_childs = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodeChilds.Length * sizeof(int)), nodeChilds.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var node_child_offset = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodeChildsOffset.Length * sizeof(int)), nodeChildsOffset.AsSpan(), &err);
		if (Failed |= err != 0) return;
		var node_child_count = _cl.CreateBuffer(_context, MemFlags.ReadOnly, (nuint)(nodeChildsCount.Length * sizeof(int)), nodeChildsCount.AsSpan(), &err);
		if (Failed |= err != 0) return;

		//copy data over, set arguments
		//    float4 parameters /*first is edge length, second attraction, third repulsion
		//						  fourth gravity */,
		//	  __global float4 *node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
		//	  __global float4 *new_node_pos /* first 2 contain pos */,
		//	  __global int* node_parents /*contains the indices of a nodes parents*/,
		//	  __global int* node_parent_offset /*starting index for parents for a node */,
		//	  __global int* node_parent_count /*count of the parents for each node */,
		//	  __global int* node_childs /*contains the indices of a nodes childs */,
		//	  __global int* node_child_offset /*starting index for childs for a node */,
		//	  __global int* node_child_count /*count of the childs for each node */,
		//	  __local float4* node_buffer /*the forces for each node*/)
		//


		//enqueue execution

		//wait on data

		//finished => take it out of the return channel and run computation again, positions are kept in gpu if nothing changed in node count

		//copy values over to our nodes
	}

	private void CreateProgram()
	{//get context with selected devices
		_context = _cl.CreateContext(null, 1, _device, null, null, out int err);
		if (Failed |= err != 0) return;

		//get context with selected devices
		_commandQueue = _cl.CreateCommandQueue(_context, _device, CommandQueueProperties.None, out err);
		if (Failed |= err != 0) return;

		//build program
		byte[] codeBytes = new byte[Encoding.Default.GetByteCount(Kernels.LayoutKernel) + 1];
		Encoding.Default.GetBytes(Kernels.LayoutKernel, 0, Kernels.LayoutKernel.Length, codeBytes, 0);
		fixed (byte* code = codeBytes)
		{
			_program = _cl.CreateProgramWithSource(_context, 1, in code, null, out err);
			if (Failed |= err != 0) return;
		}

		//get build status as it is not in the error
		nint pdevice = _device;
		int status = _cl.BuildProgram(_program, 1, &pdevice, 0, null, null);
		if (status != 0)
		{
			_ = _cl.GetProgramBuildInfo(_program, _device, ProgramBuildInfo.BuildLog, 0, null, out nuint logSize);
			sbyte[] log = new sbyte[logSize];
			fixed (sbyte* plog = log)
			{
				_ = _cl.GetProgramBuildInfo(_program, _device, ProgramBuildInfo.BuildLog, logSize, plog, null);
				LogManager.Log($"kernel build failed on {DeviceName}: \n" + new string(plog), LogManager.Level.Error);
				Failed = true;
			}
		}
	}

	private nint SelectDevice()
	{
		//prepare values for selection
		string[] deviceNames = new string[Platforms.Count];
		nint[] platformIds = new nint[Platforms.Count];
		for (int i = 0; i < Platforms.Count; i++)
		{
			deviceNames[i] = Platforms.Values.ElementAt(i).platformName;
			platformIds[i] = Platforms.Keys.ElementAt(i);
		}
		//get selection from user
		var selector = new DeviceSelection(deviceNames, Platforms[preselectedPlatform].platformName);
		DialogResult result = (DialogResult)(App.MainForm.Explorer?.Invoke(() => selector.ShowDialog(parent))!);
		if (result == DialogResult.Cancel) return nint.Zero;

		//work with it
		SelectedPlatform = platformIds[selector.SelectedDeviceIndex];
		return Platforms[SelectedPlatform].deviceId;
	}

	private void FindOpenCLPlatforms()
	{
		int err = _cl.GetPlatformIDs(0, null, out uint platformCount);
		if (Failed |= err != 0) return;

		nint[] _platforms = new nint[platformCount];
		Debug.WriteLine($"Found {platformCount} opencl platforms");

		err = _cl.GetPlatformIDs(platformCount, _platforms, Array.Empty<uint>());
		if (Failed |= err != 0) return;

		//get devices
		//platform id is key
		nuint maxValue = 0;
		for (int i = 0; i < platformCount; i++)
		{
			//get length/size of name
			err = _cl.GetPlatformInfo(_platforms[i], PlatformInfo.Name, 0, null, out nuint NameLength);
			if (Failed |= err != 0) return;
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
			err = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, 0, null, out uint deviceCount);
			if (Failed |= err != 0) return;
			Debug.WriteLine($"        this platform has: {deviceCount} opencl devices");
			nint[] _devices = new nint[deviceCount];
			//get devices
			err = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, deviceCount, _devices, Array.Empty<uint>());
			if (Failed |= err != 0) return;
			//find device with most power, opencl compute units
			for (int k = 0; k < deviceCount; k++)
			{
				nuint maxComputeUnits = 0;
				err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.MaxWorkGroupSize, (nuint)sizeof(nuint), &maxComputeUnits, out _);
				if (Failed |= err != 0) return;

				//get length/size of name
				err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.Name, 0, null, out nuint DeviceNameLength);
				if (Failed |= err != 0) return;
				string deviceNameString = string.Empty;
				if (DeviceNameLength > 0)
				{
					sbyte[] deviceName = new sbyte[DeviceNameLength + 1];
					//get device name
					fixed (sbyte* s = deviceName)
					{
						err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.Name, NameLength, &s, out _);
						if (err == 0)
							deviceNameString = new(s);
					}
				}

				Debug.WriteLine($"            {deviceNameString} with {maxComputeUnits} workgroupsize");
				if (maxComputeUnits > maxValue)
				{
					Debug.WriteLine("        >>>>chose the one above");
					maxValue = maxComputeUnits;
					preselectedPlatform = _platforms[i];
				}
				Platforms.Add(_platforms[i], (_devices[k], platformNameString + (deviceNameString.Length > 0 ? (" " + deviceNameString) : string.Empty)));
			}
		}
		//now we should have the best device
		Debug.WriteLine($"suggested device {Platforms[preselectedPlatform].platformName}");
	}
}