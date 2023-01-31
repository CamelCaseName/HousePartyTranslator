﻿using Silk.NET.OpenCL;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using Translator.Core;

namespace Translator.Explorer.OpenCL;
[SupportedOSPlatform("windows")]
internal sealed unsafe class OpenCLManager
{
	public bool OpenCLDevicePresent = false;
	private readonly CL _cl;
	private readonly string KernelName = "layout_kernel";
	private readonly Form parent;
	//key is pointer to platform
	private readonly Dictionary<nint, (nint deviceId, string platformName)> Platforms = new();
	private readonly NodeProvider Provider;

	//pointer to commandqueue for our selected device
	private nint _commandQueue;
	//pointer to context
	private nint _context;
	private nint _device;
	private nint _kernel;
	private nint _program;

	private string DeviceName = string.Empty;
	private bool Failed = false;
	private nint preselectedPlatform = 0;//pointer to preselected platform
	private nint SelectedPlatform = 0;
	private uint MaxWorkGroupSize, MaxWorkItems;

	public OpenCLManager(Form parent, NodeProvider provider)
	{
		Provider = provider;
		this.parent = parent;
		//get api and context
		_cl = CL.GetApi();
	}

	~OpenCLManager()
	{
		ReleaseOpenCLResources();
	}

	public void SetUpOpenCL()
	{
		int err = SetUpOpenCLImpl();
		if (Failed = err != 0)
		{
			ReleaseOpenCLResources();
			LogManager.Log($"Opencl failed with code {GetOpenCLErrorName(err)}, maybe see log above?", LogManager.Level.Error);
		}
	}

	private int CreateProgram()
	{//get context with selected devices
		_context = _cl.CreateContext(null, 1, _device, null, null, out int err);
		if (err != 0) return err;

		//get context with selected devices
		_commandQueue = _cl.CreateCommandQueue(_context, _device, CommandQueueProperties.None, out err);
		if (err != 0) return err;

		//build program
		byte[] codeBytes = new byte[Encoding.Default.GetByteCount(Kernels.LayoutKernel) + 1];
		Encoding.Default.GetBytes(Kernels.LayoutKernel, 0, Kernels.LayoutKernel.Length, codeBytes, 0);
		fixed (byte* code = codeBytes)
		{
			_program = _cl.CreateProgramWithSource(_context, 1, in code, null, out err);
			if (err != 0) return err;
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
		return 0;
	}

	private int FindOpenCLPlatforms()
	{
		int err = _cl.GetPlatformIDs(0, null, out uint platformCount);
		if (err != 0) return err;

		nint[] _platforms = new nint[platformCount];
		Debug.WriteLine($"Found {platformCount} opencl platforms");

		err = _cl.GetPlatformIDs(platformCount, _platforms, Array.Empty<uint>());
		if (err != 0) return err;

		//get devices
		//platform id is key
		nuint maxValue = 0;
		for (int i = 0; i < platformCount; i++)
		{
			//get length/size of name
			err = _cl.GetPlatformInfo(_platforms[i], PlatformInfo.Name, 0, null, out nuint NameLength);
			if (err != 0) return err;
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
			if (err != 0) return err;
			Debug.WriteLine($"        this platform has: {deviceCount} opencl devices");
			nint[] _devices = new nint[deviceCount];
			//get devices
			err = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, deviceCount, _devices, Array.Empty<uint>());
			if (err != 0) return err;
			//find device with most power, opencl compute units
			for (int k = 0; k < deviceCount; k++)
			{
				nuint maxComputeUnits = 0;
				err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.MaxWorkGroupSize, (nuint)sizeof(nuint), &maxComputeUnits, out _);
				if (err != 0) return err;

				//get length/size of name
				err = _cl.GetDeviceInfo(_devices[k], DeviceInfo.Name, 0, null, out nuint DeviceNameLength);
				if (err != 0) return err;
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
		return 0;
	}

	private static string GetOpenCLErrorName(int error)
	{
		return error switch
		{
			// run-time and JIT compiler errors
			0 => "CL_SUCCESS",
			-1 => "CL_DEVICE_NOT_FOUND",
			-2 => "CL_DEVICE_NOT_AVAILABLE",
			-3 => "CL_COMPILER_NOT_AVAILABLE",
			-4 => "CL_MEM_OBJECT_ALLOCATION_FAILURE",
			-5 => "CL_OUT_OF_RESOURCES",
			-6 => "CL_OUT_OF_HOST_MEMORY",
			-7 => "CL_PROFILING_INFO_NOT_AVAILABLE",
			-8 => "CL_MEM_COPY_OVERLAP",
			-9 => "CL_IMAGE_FORMAT_MISMATCH",
			-10 => "CL_IMAGE_FORMAT_NOT_SUPPORTED",
			-11 => "CL_BUILD_PROGRAM_FAILURE",
			-12 => "CL_MAP_FAILURE",
			-13 => "CL_MISALIGNED_SUB_BUFFER_OFFSET",
			-14 => "CL_EXEC_STATUS_ERROR_FOR_EVENTS_IN_WAIT_LIST",
			-15 => "CL_COMPILE_PROGRAM_FAILURE",
			-16 => "CL_LINKER_NOT_AVAILABLE",
			-17 => "CL_LINK_PROGRAM_FAILURE",
			-18 => "CL_DEVICE_PARTITION_FAILED",
			-19 => "CL_KERNEL_ARG_INFO_NOT_AVAILABLE",
			// compile-time errors
			-30 => "CL_INVALID_VALUE",
			-31 => "CL_INVALID_DEVICE_TYPE",
			-32 => "CL_INVALID_PLATFORM",
			-33 => "CL_INVALID_DEVICE",
			-34 => "CL_INVALID_CONTEXT",
			-35 => "CL_INVALID_QUEUE_PROPERTIES",
			-36 => "CL_INVALID_COMMAND_QUEUE",
			-37 => "CL_INVALID_HOST_PTR",
			-38 => "CL_INVALID_MEM_OBJECT",
			-39 => "CL_INVALID_IMAGE_FORMAT_DESCRIPTOR",
			-40 => "CL_INVALID_IMAGE_SIZE",
			-41 => "CL_INVALID_SAMPLER",
			-42 => "CL_INVALID_BINARY",
			-43 => "CL_INVALID_BUILD_OPTIONS",
			-44 => "CL_INVALID_PROGRAM",
			-45 => "CL_INVALID_PROGRAM_EXECUTABLE",
			-46 => "CL_INVALID_KERNEL_NAME",
			-47 => "CL_INVALID_KERNEL_DEFINITION",
			-48 => "CL_INVALID_KERNEL",
			-49 => "CL_INVALID_ARG_INDEX",
			-50 => "CL_INVALID_ARG_VALUE",
			-51 => "CL_INVALID_ARG_SIZE",
			-52 => "CL_INVALID_KERNEL_ARGS",
			-53 => "CL_INVALID_WORK_DIMENSION",
			-54 => "CL_INVALID_WORK_GROUP_SIZE",
			-55 => "CL_INVALID_WORK_ITEM_SIZE",
			-56 => "CL_INVALID_GLOBAL_OFFSET",
			-57 => "CL_INVALID_EVENT_WAIT_LIST",
			-58 => "CL_INVALID_EVENT",
			-59 => "CL_INVALID_OPERATION",
			-60 => "CL_INVALID_GL_OBJECT",
			-61 => "CL_INVALID_BUFFER_SIZE",
			-62 => "CL_INVALID_MIP_LEVEL",
			-63 => "CL_INVALID_GLOBAL_WORK_SIZE",
			-64 => "CL_INVALID_PROPERTY",
			-65 => "CL_INVALID_IMAGE_DESCRIPTOR",
			-66 => "CL_INVALID_COMPILER_OPTIONS",
			-67 => "CL_INVALID_LINKER_OPTIONS",
			-68 => "CL_INVALID_DEVICE_PARTITION_COUNT",
			// extension errors
			-1000 => "CL_INVALID_GL_SHAREGROUP_REFERENCE_KHR",
			-1001 => "CL_PLATFORM_NOT_FOUND_KHR",
			-1002 => "CL_INVALID_D3D10_DEVICE_KHR",
			-1003 => "CL_INVALID_D3D10_RESOURCE_KHR",
			-1004 => "CL_D3D10_RESOURCE_ALREADY_ACQUIRED_KHR",
			-1005 => "CL_D3D10_RESOURCE_NOT_ACQUIRED_KHR",
			_ => "Unknown OpenCL error",
		};
	}

	private void ReleaseOpenCLResources()
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
		DialogResult result = (DialogResult)App.MainForm.Explorer?.Invoke(() => selector.ShowDialog(parent))!;
		if (result == DialogResult.Cancel) return nint.Zero;

		//work with it
		SelectedPlatform = platformIds[selector.SelectedDeviceIndex];
		return Platforms[SelectedPlatform].deviceId;
	}

	private int SetUpOpenCLImpl()
	{
		//get platform
		int err = FindOpenCLPlatforms();
		if (err != 0) return err;

		//just do the old way if we have no opencl
		if (Platforms.Count == 0) return -1;
		OpenCLDevicePresent = true;

		_device = SelectDevice();
		if (_device == nint.Zero) return -1;

		DeviceName = Platforms[SelectedPlatform].platformName;

		//create contet, program and build it on the selected device
		err = CreateProgram();
		if (err != 0) return err;
		//success, we can go on creating the kernel
		_kernel = _cl.CreateKernel(_program, KernelName, out err);
		if (err != 0) return err;

		err = GetMaxWorkItemDimension();
		if (err != 0) return err;

		//create and fill buffers on cpu side
		var nodePositionBuffer = Provider.GetNodePositionBuffer();
		var returnedNodePositionBuffer = Provider.GetNodeNewPositionBuffer();
		(int[] nodeParents, int[] nodeParentsOffset, int[] nodeParentsCount) = Provider.GetNodeParentsBuffer();
		(int[] nodeChilds, int[] nodeChildsOffset, int[] nodeChildsCount) = Provider.GetNodeChildsBuffer();
		var parameters = new float[4] { StoryExplorerConstants.IdealLength, StoryExplorerConstants.Attraction, StoryExplorerConstants.Repulsion, StoryExplorerConstants.Gravity };

		//calculate work size for local stuff
		nuint neededWorkSize = (nuint)((Math.Sqrt(Provider.Nodes.Count)) + 0.5d);
		if (neededWorkSize > MaxWorkGroupSize) return -1;
		nuint neededWorkitems = (nuint)(Provider.Nodes.Count) / neededWorkSize;
		//divisible by 4 for nice boundaries
		while ((neededWorkSize) % 4 > 0) ++neededWorkSize;
		while ((neededWorkitems) % 4 > 0) ++neededWorkitems;

		//create buffers on gpu
		// we can quickly swap buffers and start the calculations again if we want
		var node_pos_1 = _cl.CreateBuffer(_context, MemFlags.ReadWrite | MemFlags.CopyHostPtr, (nuint)(nodePositionBuffer.Length * sizeof(float) * 4), nodePositionBuffer.AsSpan(), &err);
		if (err != 0) return err;
		var node_pos_2 = _cl.CreateBuffer(_context, MemFlags.ReadWrite | MemFlags.CopyHostPtr, (nuint)(returnedNodePositionBuffer.Length * sizeof(float) * 4), returnedNodePositionBuffer.AsSpan(), &err);
		if (err != 0) return err;

		//flat linked list edge representation
		var node_parents = _cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, (nuint)(nodeParents.Length * sizeof(int)), nodeParents.AsSpan(), &err);
		if (err != 0) return err;
		var node_parent_offset = _cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, (nuint)(nodeParentsOffset.Length * sizeof(int)), nodeParentsOffset.AsSpan(), &err);
		if (err != 0) return err;
		var node_parent_count = _cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, (nuint)(nodeParentsCount.Length * sizeof(int)), nodeParentsCount.AsSpan(), &err);
		if (err != 0) return err;
		var node_childs = _cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, (nuint)(nodeChilds.Length * sizeof(int)), nodeChilds.AsSpan(), &err);
		if (err != 0) return err;
		var node_child_offset = _cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, (nuint)(nodeChildsOffset.Length * sizeof(int)), nodeChildsOffset.AsSpan(), &err);
		if (err != 0) return err;
		var node_child_count = _cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, (nuint)(nodeChildsCount.Length * sizeof(int)), nodeChildsCount.AsSpan(), &err);
		if (err != 0) return err;

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
		err = _cl.SetKernelArg<float>(_kernel, 0, sizeof(float) * 4, parameters.AsSpan());
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 1, (nuint)sizeof(nint), node_pos_1);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 2, (nuint)sizeof(nint), node_pos_2);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 3, (nuint)sizeof(nint), node_parents);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 4, (nuint)sizeof(nint), node_parent_offset);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 5, (nuint)sizeof(nint), node_parent_count);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 6, (nuint)sizeof(nint), node_childs);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 7, (nuint)sizeof(nint), node_child_offset);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 8, (nuint)sizeof(nint), node_child_count);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 9, (nuint)sizeof(nint), null);
		if (err != 0) return err;

		//enqueue execution, same method as for the loop later
		err = DoKernelCalculation(node_pos_1, node_pos_2, neededWorkitems, neededWorkSize);
		if (err != 0) return err;

		_cl.Finish(_commandQueue);
		//enqueue read out of results
		fixed (float* buffer = returnedNodePositionBuffer)
		{
			//finished => take it out of the return channel and run computation again, positions are kept in gpu if nothing changed in node count
			err = _cl.EnqueueReadBuffer(_commandQueue, node_pos_1, false, 0, (nuint)(returnedNodePositionBuffer.Length * sizeof(float) * 4), buffer, 0, null, null);
			if (err != 0) return err;

			//copy values over to our nodes
			Provider.SetNewNodePositions(returnedNodePositionBuffer);
			return 0;
		}
	}

	//does not read out the data
	private int DoKernelCalculation(nint node_pos_1, nint node_pos_2, nuint neededWorkItems, nuint neededWorkSize)
	{
		int err;
		//we cant have too many
		//if (neededWorkItems * neededWorkSize > MaxWorkGroupSize) return -1;

		//enqueue kernel with old positions
		err = _cl.EnqueueNdrangeKernel(_commandQueue, _kernel, 1, 0, neededWorkItems, neededWorkSize, 0, null, null);
		if (err != 0) return err;

		//set args for next call
		err = _cl.SetKernelArg(_kernel, 1, (nuint)sizeof(nint), node_pos_1);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 2, (nuint)sizeof(nint), node_pos_2);
		if (err != 0) return err;

		//enqueue a second time so our arguments stay equal
		err = _cl.EnqueueNdrangeKernel(_commandQueue, _kernel, 1, 0, neededWorkItems, neededWorkSize, 0, null, null);
		if (err != 0) return err;

		//set args for next call
		err = _cl.SetKernelArg(_kernel, 1, (nuint)sizeof(nint), node_pos_2);
		if (err != 0) return err;
		err = _cl.SetKernelArg(_kernel, 2, (nuint)sizeof(nint), node_pos_1);
		if (err != 0) return err;

		return 0;
	}

	private int GetMaxWorkItemDimension()
	{
		int err;
		nuint valueToGet = 0;
		err = _cl.GetDeviceInfo(Platforms[SelectedPlatform].deviceId, DeviceInfo.MaxWorkGroupSize, (nuint)sizeof(nuint), &valueToGet, out _);
		if (err != 0) return err;
		MaxWorkGroupSize = (uint)valueToGet;
		uint[] valuesToGet = new uint[32];
		fixed (uint* value = valuesToGet)
		{
			err = _cl.GetDeviceInfo(Platforms[SelectedPlatform].deviceId, DeviceInfo.MaxWorkItemSizes, (nuint)(sizeof(nuint) * valuesToGet.Length), value, out _);
		}
		if (err != 0) return err;
		MaxWorkItems = valuesToGet[0];
		return 0;
	}
}