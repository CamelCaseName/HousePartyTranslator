﻿using System.Numerics;
using System.Runtime.Versioning;
using System.Windows.Forms.VisualStyles;
using Translator.Core;
using Translator.Explorer.OpenCL;

namespace Translator.Explorer
{
    internal static class StoryExplorerConstants
    {
        public const float Attraction = 0.3f;//Attraction accelleration multiplier, between 0 and 1
        public const float Repulsion = 70.0f;//Repulsion accelleration multiplier, between 0 and much
        public const float Gravity = 0.0001f;
        public static float IdealLength = 100; //spring IdealLength in units aka thedistance an edge should be long
        public static int ColoringDepth = 15;
        public static int Nodesize = 16;
    }

    [SupportedOSPlatform("windows")]
    internal sealed class NodeLayout
    {
        private List<Vector2> NodeForces = new();
        private DateTime StartTime = DateTime.MinValue;
        private DateTime FrameStartTime = DateTime.MinValue;
        private DateTime FrameEndTime = DateTime.MinValue;
        public bool RunOverride = false;
        private CancellationTokenSource cancellationToken = new();
        private readonly CancellationToken outsideToken;
        private readonly OpenCLManager opencl;
        private readonly NodeProvider provider;
        private readonly Action LayoutCalculation;
        private int _framecount = 0;
        public int FrameCount => _framecount;
        public bool Finished => FrameCount > (long)Nodes.Count * (long)Nodes.Count && !RunOverride && Started;
        public bool Started { get; private set; } = false;
        public NodeList Nodes
        {
            get { return provider.Nodes; }
        }
        private NodeList Internal
        {
            get { return provider.OtherNodes; }
        }

        public NodeLayout(NodeProvider provider, Form parent, CancellationToken cancellation)
        {
            outsideToken = cancellation;
            this.provider = provider;

            LayoutCalculation = () => CalculateForceDirectedLayout(cancellationToken.Token);
            
            opencl = new(parent, provider);
            //its not worth it for less nodes
            if (Nodes.Count >= 1024)
            {
                opencl.SetUpOpenCL();
                if (opencl.OpenCLDevicePresent && !opencl.Failed)
                {
                    LayoutCalculation = () => opencl.CalculateLayout(() => ++_framecount, cancellationToken.Token);
                }
                else
                {
                    //clear memory
                    opencl.ReleaseOpenCLResources();
                    opencl = null!;
                }
            }
            else
            {
                //clear memory
                opencl.ReleaseOpenCLResources();
                opencl = null!;
            }
        }

        public void Start()
        {
            if (!Started)
            {
                cancellationToken = new();
                _ = outsideToken.Register(() => cancellationToken.Cancel());
                StartTime = DateTime.Now;
                LogManager.Log($"\tnode layout started for {Nodes.Count} nodes");
                Started = true;
                _ = Task.Run(LayoutCalculation, cancellationToken.Token);
            }
        }

        public void Stop()
        {
            DateTime end = DateTime.Now;
            LogManager.Log($"\tnode layout ended, rendered {FrameCount} frames in {(end - StartTime).TotalSeconds:F2} seconds -> {FrameCount / (end - StartTime).TotalSeconds:F2} fps");
            cancellationToken.Cancel();
            Started = false;
            _framecount = 0;
        }

        public void CalculateForceDirectedLayout(CancellationToken token)
        {
            //save all forces here
            NodeForces = new(Nodes.Count);
            Nodes.ForEach(n => NodeForces.Add(new Vector2()));

            while (!token.IsCancellationRequested && !Finished)
            {
                FrameStartTime = FrameEndTime;

                //calculate
                CalculatePositions();

                //we got to wait before we change nodes, so like a reverse lock?
                while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
                //switch to other list once done
                provider.UsingListA = !provider.UsingListA;
                ++_framecount;

                //approx 40fps max as more is uneccesary and feels weird (25ms gives ~50fps, 30 gives about 45fps)
                FrameEndTime = DateTime.Now;
                if ((FrameEndTime - FrameStartTime).TotalMilliseconds < 30) Thread.Sleep((int)(30 - (FrameEndTime - FrameStartTime).TotalMilliseconds));

                App.MainForm?.Explorer?.Invalidate();
            }
            App.MainForm?.Explorer?.Invoke(() => App.MainForm?.Explorer?.Stop_Click(new(), EventArgs.Empty));
            Stop();
        }

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
                    pos.X = 0.1f;
                    pos.Y = 0.1f;
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

                        //Repulsion
                        NodeForces[first] += (edge / edge.LengthSquared()) * StoryExplorerConstants.Repulsion;
                        NodeForces[second] -= (edge / edge.LengthSquared()) * StoryExplorerConstants.Repulsion;
                    }
                    else
                    {
                        //move away
                        Internal[first].Position.X += 10f;
                    }
                }
            }

            for (int i = 0; i < Internal.Edges.Count; i++)
            {
                //Attraction/spring accelleration on edge
                Vector2 edge = new(
                        Internal.Edges[i].This.Position.X - Internal.Edges[i].Child.Position.X,
                        Internal.Edges[i].This.Position.Y - Internal.Edges[i].Child.Position.Y
                        );
                Vector2 attractionVec = (edge / edge.Length()) * StoryExplorerConstants.Attraction * (edge.Length() - StoryExplorerConstants.IdealLength);

                    NodeForces[Internal.Edges[i].ThisIndex] -= attractionVec / Internal.Edges[i].This.Mass;
                    NodeForces[Internal.Edges[i].ChildIndex] += attractionVec / Internal.Edges[i].Child.Mass;
            }

            //apply accelleration to nodes
            for (int i = 0; i < Internal.Count; i++)
            {
                if (!Internal[i].IsPositionLocked)
                {
                    Internal[i].Position.X += NodeForces[i].X;
                    Internal[i].Position.Y += NodeForces[i].Y;
                }
            }
        }
    }
}
