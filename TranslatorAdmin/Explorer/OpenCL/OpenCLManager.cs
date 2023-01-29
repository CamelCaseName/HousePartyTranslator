using Silk.NET.OpenCL;
using System.Diagnostics;
using System.Runtime.Versioning;
using Translator.Core;

namespace Translator.Explorer.OpenCL;
[SupportedOSPlatform("windows")]
internal sealed unsafe class OpenCLManager
{
	private nint preselectedPlatform = 0;//pointer to preselected platform
	private nint SelectedPlatform = 0;
	private readonly CL _cl;
	private readonly Dictionary<nint, (nint deviceId, string platformName)> Platforms = new();//key is pointer to platform
	private readonly string DeviceName = string.Empty;
	private string KernelName = string.Empty;
	private nint _context;//pointer to context
	private nint _commandQueue;//pointer to commandqueue for our selected device
	private readonly nint _device;
	private nint _program;
	private readonly nint _kernel;
	public bool OpenCLDevicePresent = false;
	private readonly Form parent;

	public OpenCLManager(Form parent)
	{
		this.parent = parent;
		//get api and context
		_cl = CL.GetApi();

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

		//success, we can go on creating the kernel
		_kernel = _cl.CreateKernel(_program, KernelName, out int err);
		Debug.Assert(err == 0);
	}

	private void CreateProgram()
	{//get context with selected devices
		_context = _cl.CreateContext(null, 1, _device, null, null, out int err);
		Debug.Assert(err == 0);

		//get context with selected devices
		_commandQueue = _cl.CreateCommandQueue(_context, _device, CommandQueueProperties.None, out err);
		Debug.Assert(err == 0);

		//build program
		string KernelCode = Kernels.TestKernel;
		KernelName = nameof(Kernels.TestKernel);
		nuint KernelCodeLength = (nuint)(KernelCode.Length * 2);
		_program = _cl.CreateProgramWithSource(_context, 1, new string[] { KernelCode }, &KernelCodeLength, out err);
		Debug.Assert(err == 0);

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
		Debug.Assert(err == 0);

		nint[] _platforms = new nint[platformCount];
		Debug.WriteLine($"Found {platformCount} opencl platforms");

		err = _cl.GetPlatformIDs(platformCount, _platforms, Array.Empty<uint>());
		Debug.Assert(err == 0);

		//get devices
		//platform id is key
		nuint maxValue = 0;
		for (int i = 0; i < platformCount; i++)
		{
			//get length/size of name
			err = _cl.GetPlatformInfo(_platforms[i], PlatformInfo.Name, 0, null, out nuint NameLength);
			Debug.Assert(err == 0);
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
			Debug.Assert(err == 0);
			Debug.WriteLine($"        this platform has: {deviceCount} opencl devices");
			nint[] _devices = new nint[deviceCount];
			//get devices
			err = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, deviceCount, _devices, Array.Empty<uint>());
			Debug.Assert(err == 0);
			//find device with most power, opencl compute units
			for (int k = 0; k < deviceCount; k++)
			{
				nuint maxComputeUnits = 0;
				err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.MaxWorkGroupSize, (nuint)sizeof(nuint), &maxComputeUnits, out _);
				Debug.Assert(err == 0);

				//get length/size of name
				err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.Name, 0, null, out nuint DeviceNameLength);
				Debug.Assert(err == 0);
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