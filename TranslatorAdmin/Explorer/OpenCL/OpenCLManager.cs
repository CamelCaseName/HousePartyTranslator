﻿using Silk.NET.OpenCL;
using System.Runtime.Versioning;
using System.Text;
using Translator.Core;
using WinUtils = Translator.Core.Helpers.Utils<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;


namespace Translator.Explorer.OpenCL;
[SupportedOSPlatform("windows")]
internal sealed unsafe class OpenCLManager
{
    public bool Failed { get; private set; } = false;
    public bool Retry { get; set; } = false;
    public bool OpenCLDevicePresent = false;
    private readonly CL _cl;
    private readonly string NBodyKernelName = "nbody_kernel";
    private readonly string EdgeKernelName = "edge_kernel";
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

    private bool ResourcesAreAcquired = false;
    private float[] nodePosBuffer1 = Array.Empty<float>(), nodePosBuffer2 = Array.Empty<float>(), nodePosResultBuffer = Array.Empty<float>();
    private readonly List<int> parent_indices = new();
    private readonly List<int> parent_offset = new();
    private readonly List<int> parent_count = new();
    private readonly List<int> child_indices = new();
    private readonly List<int> child_offset = new();
    private readonly List<int> child_count = new();
    private int NodeCount = 0;
    private nint node_pos_1, node_pos_2;
    private nint preselectedPlatform = 0;//pointer to preselected platform
    private nint SelectedPlatform = 0;
    private nuint neededGlobalSize, neededLocalSize;
    private string DeviceName = string.Empty;
    private uint MaxWorkGroupSize, MaxWorkItems, PreferredLocalWorkSize;
    private DateTime FrameStartTime = DateTime.MinValue;
    private DateTime FrameEndTime = DateTime.MinValue;

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
        byte[] codeBytes = new byte[Encoding.Default.GetByteCount(Kernels.NBodyKernel) + 1];
        Encoding.Default.GetBytes(Kernels.NBodyKernel, 0, Kernels.NBodyKernel.Length, codeBytes, 0);
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
            }

            if (platformNameString.Contains("CPU")) continue;

            //get device count
            err = _cl.GetDeviceIDs(_platforms[i], DeviceType.Gpu, 0, null, out uint deviceCount);
            if (err != 0) return err;
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

                if (maxComputeUnits > maxValue)
                {
                    maxValue = maxComputeUnits;
                    preselectedPlatform = _platforms[i];
                }
                Platforms.Add(_platforms[i], (_devices[k], platformNameString));
            }
        }
        //now we should have the best device
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
        try
        {
            if (_commandQueue != nint.Zero)
                _cl.Finish(_commandQueue);
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
        catch (Exception e)
        {
            LogManager.Log(e.ToString(), LogManager.Level.Error);

            WinUtils.DisplayExceptionMessage(e.Message);
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

        if (_device == nint.Zero) _device = SelectDevice();
        if (_device == nint.Zero) return -1;

        DeviceName = Platforms[SelectedPlatform].platformName;

        //get all resources we need, context kernel and so on
        err = AcquireRessources();
        if (err != 0) return err;

        err = SetUpBuffers();
        if (err != 0) return err;

        //do the calculation once
        return CalculateAndCopy(nodePosResultBuffer);
    }

    private int SetUpBuffers()
    {
        int err;

        //todo add method to redo the buffers once node count changes and so on, once nodes and edges get added
        //maybe do like a buffer bool, so create bigger initially and then limit length, idk

        //create and fill buffers on cpu side
        SetUpNodePositionBuffer();
        ClearNodeNewPositionBuffer();
        var parameters = new float[4] { StoryExplorerConstants.IdealLength, StoryExplorerConstants.OpenCLAttraction, StoryExplorerConstants.Repulsion / 2, StoryExplorerConstants.OpenClGravity };
        NodeCount = nodePosBuffer1.Length / 4;

        //calculate work size for local stuff
        neededLocalSize = (nuint)((Math.Sqrt(NodeCount)) + 0.5d);
        if (neededLocalSize > PreferredLocalWorkSize) neededLocalSize = PreferredLocalWorkSize;
        else while ((neededLocalSize) % 4 > 0) ++neededLocalSize;
        if (neededLocalSize > MaxWorkGroupSize) return -1;

        //calculate item count
        neededGlobalSize = (nuint)NodeCount;
        //divisible by 4 for nice boundaries
        while ((neededGlobalSize) % neededLocalSize > 0) ++neededGlobalSize;

        //create buffers on gpu
        // we can quickly swap buffers and start the calculations again if we want
        node_pos_1 = _cl.CreateBuffer(_context, MemFlags.ReadWrite | MemFlags.CopyHostPtr, (nuint)(nodePosBuffer1.Length * sizeof(float) * 4), nodePosBuffer1.AsSpan(), &err);
        if (err != 0) return err;
        node_pos_2 = _cl.CreateBuffer(_context, MemFlags.ReadWrite | MemFlags.CopyHostPtr, (nuint)(nodePosBuffer2.Length * sizeof(float) * 4), nodePosBuffer2.AsSpan(), &err);
        if (err != 0) return err;

        //copy data over, set arguments
        //    float4 parameters /*first is edge length, second attraction, third repulsion
        //						  fourth gravity */,
        //	  __global float4 *node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
        //	  __global float4 *new_node_pos /* first 2 contain pos */,
        //    __global int actual_node_count /*actual count of nodes*/
        //	  __local float4* node_buffer /*the forces for each node*/)
        //
        err = _cl.SetKernelArg<float>(_kernel, 0, sizeof(float) * 4, parameters.AsSpan());
        if (err != 0) return err;
        err = _cl.SetKernelArg(_kernel, 3, sizeof(int), NodeCount);
        if (err != 0) return err;
        err = _cl.SetKernelArg(_kernel, 4, sizeof(float) * 4 * neededLocalSize, null);
        return err;
    }

    private int CalculateAndCopy(float[] resultBuffer)
    {
        int err;

        //enqueue read out of results, add new positions/position lockouts
        //set manual pos or node locked states have changed, so only if necessary, minimise pcie data transfers
        if (Provider.MovingNodePositionOverridden)
        {
            //update/override moving node position in last result buffer, so we can use it as new base without reading out twice
            (int index, float x, float y) = Provider.MovingNodeInfo;
            if (index >= 0)
            {
                if (Provider.MovingNodePositionOverrideEnded)
                {
                    resultBuffer[(index * 4) + 2] = 0.0f;
                    Provider.ConsumedNodePositionOverrideEnded();
                }
                else
                {
                    resultBuffer[index * 4] = x;
                    resultBuffer[(index * 4) + 1] = y;
                    resultBuffer[(index * 4) + 2] = 1.0f;
                }

                fixed (float* inputBuffer = resultBuffer)
                {
                    err = _cl.EnqueueWriteBuffer(_commandQueue, node_pos_1, false, 0, (nuint)(resultBuffer.Length * sizeof(float) * 4), inputBuffer, 0, null, null);
                    if (err != 0) return err;
                }
            }
        }

        //enqueue execution, same method as for the loop later
        err = DoKernelCalculation(neededGlobalSize, neededLocalSize);
        if (err != 0) return err;

        fixed (float* outputBuffer = resultBuffer)
        {
            //finished => take it out of the return channel and run computation again, positions are kept in gpu if nothing changed in node count
            err = _cl.EnqueueReadBuffer(_commandQueue, node_pos_1, false, 0, (nuint)(resultBuffer.Length * sizeof(float)), outputBuffer, 0, null, null);
            if (err != 0) return err;

            //wait on all commands to finish
            err = _cl.Finish(_commandQueue);
            if (err != 0) return err;
        }

        //copy values over to our nodes
        SetNewNodePositions(resultBuffer);
        return 0;
    }

    private int DoKernelCalculation(nuint globalSize, nuint localSize)
    {
        int err;

        //copy new data to other buffer so the deltas add up correctly
        err = _cl.EnqueueCopyBuffer(_commandQueue, node_pos_1, node_pos_2, 0, 0, (nuint)(nodePosBuffer1.Length * sizeof(float) * 4), 0, null, null);
        if (err != 0) return err;

        err = _cl.Flush(_commandQueue);
        if (err != 0) return err;

        err = _cl.SetKernelArg(_kernel, 1, (nuint)sizeof(nint), node_pos_1);
        if (err != 0) return err;
        err = _cl.SetKernelArg(_kernel, 2, (nuint)sizeof(nint), node_pos_2);
        if (err != 0) return err;

        //enqueue kernel with old positions
        err = _cl.EnqueueNdrangeKernel(_commandQueue, _kernel, 1, 0, globalSize, localSize, 0, null, null);
        if (err != 0) return err;

        //second run
        err = _cl.Flush(_commandQueue);
        if (err != 0) return err;

        //copy new data to other buffer so the deltas add up correctly
        err = _cl.EnqueueCopyBuffer(_commandQueue, node_pos_2, node_pos_1, 0, 0, (nuint)(nodePosBuffer2.Length * sizeof(float) * 4), 0, null, null);
        if (err != 0) return err;

        //second run
        err = _cl.Flush(_commandQueue);
        if (err != 0) return err;

        err = _cl.SetKernelArg(_kernel, 1, (nuint)sizeof(nint), node_pos_2);
        if (err != 0) return err;
        err = _cl.SetKernelArg(_kernel, 2, (nuint)sizeof(nint), node_pos_1);
        if (err != 0) return err;

        //enqueue kernel with old positions
        err = _cl.EnqueueNdrangeKernel(_commandQueue, _kernel, 1, 0, globalSize, localSize, 0, null, null);
        if (err != 0) return err;

        //second run
        err = _cl.Flush(_commandQueue);
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
        fixed (uint* value = valuesToGet)
        {
            err = _cl.GetDeviceInfo(Platforms[SelectedPlatform].deviceId, DeviceInfo.PreferredWorkGroupSizeMultiple, (nuint)(sizeof(nuint) * valuesToGet.Length), value, out _);
        }
        if (err != 0) return err;
        PreferredLocalWorkSize = valuesToGet[0];
        return 0;
    }

    private int AcquireRessources()
    {
        Failed = !Retry;
        int err;
        //create contet, program and build it on the selected device
        err = CreateProgram();
        if (err != 0) return err;
        //success, we can go on creating the kernel
        _kernel = _cl.CreateKernel(_program, NBodyKernelName, out err);
        if (err != 0) return err;

        ResourcesAreAcquired = true;

        err = GetMaxWorkItemDimension();
        return err;
    }

    internal void CalculateLayout(Action? FrameRenderedCallback, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            FrameStartTime = FrameEndTime;

            if (Provider.OtherNodes.Count == NodeCount && ResourcesAreAcquired && !Failed)
            {
                //nothing changed regarding node count, we can keep as is and dont have to redo the buffers and so on
                CalculateAndCopy(nodePosResultBuffer);

                //we got to wait before we change nodes, so like a reverse lock?
                while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
                //switch to other list once done
                Provider.UsingListA = !Provider.UsingListA;
                FrameRenderedCallback?.Invoke();

                //approx 60fps max as more is uneccesary and feels weird
                FrameEndTime = DateTime.Now;
                if ((FrameEndTime - FrameStartTime).TotalMilliseconds < 30) Thread.Sleep((int)(30 - (FrameEndTime - FrameStartTime).TotalMilliseconds));

                App.MainForm?.Explorer?.Invalidate();
            }
            else if (ResourcesAreAcquired && !Failed)
            {
                //count changed, we have to redo all the outputBuffer and size control setup
                int err = SetUpBuffers();
                if (err != 0)
                    ReleaseOpenCLResources();
                else
                    CalculateAndCopy(nodePosResultBuffer);
            }
            else if (Retry)
            {
                //just try setting up again
                _device = Platforms[SelectedPlatform].deviceId;
                int err = AcquireRessources();
                if (err != 0)
                    ReleaseOpenCLResources();
                err = SetUpBuffers();
                if (err != 0)
                    ReleaseOpenCLResources();
                CalculateAndCopy(nodePosResultBuffer);
            }
            //todo add warmup code or whatever to recover once stopped and then restarted, maybe seperate shutdown and break code
        }
        //release once were done
        ReleaseOpenCLResources();
    }

    public void SetUpNodePositionBuffer()
    {
        //cache the array so we do less allocations
        if (nodePosBuffer1.Length != Provider.OtherNodes.Count * 4)
            nodePosBuffer1 = new float[Provider.OtherNodes.Count * 4];

        //set new data
        for (int i = 0; i < Provider.OtherNodes.Count; i++)
        {
            nodePosBuffer1[i * 4] = Provider.OtherNodes[i].Position.X;
            nodePosBuffer1[(i * 4) + 1] = Provider.OtherNodes[i].Position.Y;
            nodePosBuffer1[(i * 4) + 2] = Provider.OtherNodes[i].IsPositionLocked ? 1.0f : 0.0f;
            nodePosBuffer1[(i * 4) + 3] = Provider.OtherNodes[i].Mass;
        }

        //cache the array so we do less allocations
        if (nodePosBuffer2.Length != nodePosBuffer1.Length)
            nodePosBuffer2 = new float[nodePosBuffer1.Length];

        nodePosBuffer1.CopyTo(nodePosBuffer2, 0);
    }

    public void ClearNodeNewPositionBuffer()
    {
        if (nodePosResultBuffer.Length != nodePosBuffer1.Length)
            nodePosResultBuffer = new float[nodePosBuffer1.Length];

        for (int i = 0; i < nodePosResultBuffer.Length / 4; i++)
        {
            nodePosResultBuffer[i * 4] = 0.0f;
            nodePosResultBuffer[(i * 4) + 1] = 0.0f;
            nodePosResultBuffer[(i * 4) + 2] = 0.0f;
            nodePosResultBuffer[(i * 4) + 3] = 0.0f;
        }
    }

    public (int[] indices, int[] offsets, int[] count) GetNodeParentsBuffer()
    {
        int offset = 0;
        parent_count.Clear();
        parent_indices.Clear();
        parent_offset.Clear();
        for (int i = 0; i < Provider.OtherNodes.Count; i++)
        {
            //from offset
            parent_offset.Add(offset);
            for (int j = 0; j < Provider.OtherNodes[i].ParentNodes.Count; j++)
            {
                int index = Provider.OtherNodes.IndexOf(Provider.OtherNodes[i].ParentNodes[j]);
                if (index == -1) continue;

                parent_indices.Add(index);
                offset++;
            }
            //till count are our edges
            parent_count.Add(Provider.OtherNodes[i].ParentNodes.Count);
        }

        return (parent_indices.ToArray(), parent_offset.ToArray(), parent_count.ToArray());
    }

    internal (int[] node_childs, int[] node_childs_offset, int[] node_childs_count) GetNodeChildsBuffer()
    {
        int offset = 0;
        child_count.Clear();
        child_indices.Clear();
        child_offset.Clear();
        for (int i = 0; i < Provider.OtherNodes.Count; i++)
        {
            child_offset.Add(offset);
            for (int j = 0; j < Provider.OtherNodes[i].ChildNodes.Count; j++)
            {
                int index = Provider.OtherNodes.IndexOf(Provider.OtherNodes[i].ChildNodes[j]);
                if (index == -1) continue;

                child_indices.Add(index);
                offset++;
            }
            child_count.Add(Provider.OtherNodes[i].ChildNodes.Count);
        }

        return (child_indices.ToArray(), child_offset.ToArray(), child_count.ToArray());
    }

    internal void SetNewNodePositions(float[] returnedNodePositionBuffer)
    {
        for (int i = 0; i < returnedNodePositionBuffer.Length / 4; i++)
        {
            if (returnedNodePositionBuffer[(i * 4) + 2] == 0.0f)
            {
                Provider.OtherNodes[i].Position.X = returnedNodePositionBuffer[i * 4];
                Provider.OtherNodes[i].Position.Y = returnedNodePositionBuffer[(i * 4) + 1];
            }
        }
    }
}