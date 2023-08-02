using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Desktop.Explorer.OpenCL;

namespace Translator.Desktop.Explorer.Graph
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
#if DEBUG
        private DateTime DrawStartTime = DateTime.MinValue;
#endif
        private DateTime FrameEndTime = DateTime.MinValue;
        public bool RunOverride = false;
        private CancellationTokenSource cancellationToken = new();
        private readonly CancellationToken outsideToken;
        private readonly OpenCLManager opencl;
        private readonly NodeProvider provider;
        private readonly Action LayoutCalculation;
        private int _layoutcount = 0;
        public int LayoutCount => _layoutcount;
        public bool Finished => LayoutCount > Nodes.Count * (long)Nodes.Count && !RunOverride && Started;
        public bool Started => started;
        private bool started = false;
        public NodeList Nodes
        {
            get { return provider.Nodes; }
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
                LogManager.Log("Asking for OpenCL");
                opencl.SetUpOpenCL();
                if (opencl.OpenCLDevicePresent && !opencl.Failed)
                {
                    LayoutCalculation = () => opencl.CalculateLayout(() => ++_layoutcount, cancellationToken.Token);
                }
                else
                {
                    LogManager.Log("Aborted OpenCL device selection");
                    //clear memory
                    opencl.ReleaseOpenCLResources();
                    opencl = null!;
                }
            }
            else
            {
                LogManager.Log("Dataset too small for OpenCL");
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
                _ = outsideToken.Register(cancellationToken.Cancel);
                StartTime = DateTime.Now;
                LogManager.Log($"\tnode layout started for {Nodes.Count} nodes");
                started = true;
                if (opencl is not null) opencl.Retry = true;
                _ = Task.Run(LayoutCalculation, cancellationToken.Token).ContinueWith((result) =>
                {
                    if (result.Exception is not null)
                        LogManager.Log(result.Exception!.Message, LogManager.Level.Error);
                });
            }
        }

        public void Stop()
        {
            DateTime end = DateTime.Now;
            LogManager.Log($"\tnode layout ended, calculated {LayoutCount} layouts in {(end - StartTime).TotalSeconds:F2} seconds -> {LayoutCount / (end - StartTime).TotalSeconds:F2} ups");
            cancellationToken.Cancel();
            started = false;
            _layoutcount = 0;
        }

        public void CalculateForceDirectedLayout(CancellationToken token)
        {
            //save all forces here
            if (NodeForces.Count != Nodes.Count)
            {
                NodeForces = new(Nodes.Count);
                for (int i = 0; i < NodeForces.Count; i++)
                {
                    NodeForces.Add(default);
                }
            }
            else
            {
                for (int i = 0; i < NodeForces.Count; i++)
                {
                    NodeForces[i] = default;
                }
            }

            while (!token.IsCancellationRequested && !Finished)
            {
                FrameStartTime = FrameEndTime;

                //calculate
                CalculatePositions();
#if DEBUG
                DrawStartTime = DateTime.Now;
#endif
                //its not faster to clean out this access chain!
                //we got to wait before we change nodes, so like a reverse lock?
                while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
                //switch to other list once done
                provider.UsingListA = !provider.UsingListA;
                ++_layoutcount;

                //approx 40fps max as more is uneccesary and feels weird (25ms gives ~50fps, 30 gives about 45fps)
                FrameEndTime = DateTime.Now;
                TimeSpan frametime = FrameEndTime - FrameStartTime;
#if DEBUG
                LogManager.Log($"Total: {frametime.TotalMilliseconds} Calc: {(DrawStartTime - FrameStartTime).TotalMilliseconds} %-> {(DrawStartTime - FrameStartTime).TotalMilliseconds / frametime.TotalMilliseconds * 100:000}%");
#endif
                if (frametime.TotalMilliseconds < 30) Thread.Sleep((int)(30 - frametime.TotalMilliseconds));

                App.MainForm?.Explorer?.Invalidate();
            }
            App.MainForm?.Explorer?.ShowStoppedInfoLabel();
            Stop();
        }

        private void CalculatePositions()
        {
            NodeList list = provider.OtherNodes;
            if (list.Count == 0) return;
            ResetNodeForces();

            float radius = MathF.Sqrt(list.Count) + (StoryExplorerConstants.IdealLength * 2);

            for (int first = 0; first < list.Count; first++)
            {
#if DEBUG
                if (float.IsNaN(list[first].Position.X) || float.IsNaN(list[first].Position.X)) Debugger.Break();
#endif
                //Gravity to center
                Vector2 pos = new(list[first].Position.X, list[first].Position.Y);
                float length = pos.Length();

                //fix any issues before we divide by pos IdealLength
                if (length == 0)
                {
                    pos.X = 0.1f;
                    pos.Y = 0.1f;
                }

                //can IdealLength ever be absolutely 0?
                //gravity calculation
                NodeForces[first] -= pos / length * MathF.Pow(MathF.Abs(length - radius), 1.5f) * MathF.Sign(length - radius) * StoryExplorerConstants.Gravity;

                for (int second = first + 1; second < list.Count; second++)
                {
                    if (list[first].Position != list[second].Position)
                    {
                        Vector2 edge = new(
                                list[first].Position.X - list[second].Position.X,
                                list[first].Position.Y - list[second].Position.Y
                                );

                        //Repulsion
                        NodeForces[first] += edge / edge.LengthSquared() * StoryExplorerConstants.Repulsion;
                        NodeForces[second] -= edge / edge.LengthSquared() * StoryExplorerConstants.Repulsion;
                    }
                    else
                    {
                        //move away
                        list[first].Position.X += 10f;
                    }
                }
            }

            for (int i = 0; i < list.Edges.Count; i++)
            {
                //Attraction/spring accelleration on edge
                Vector2 edge = new(
                        list.Edges[i].This.Position.X - list.Edges[i].Child.Position.X,
                        list.Edges[i].This.Position.Y - list.Edges[i].Child.Position.Y
                        );
                //if we have the exact same pos but it hasnt been moved by the general +10f in the nbody sim, we have a selfreference and can ignore it
                if (edge.X == 0 && edge.Y == 0) continue;

                Vector2 attractionVec = edge / edge.Length() * StoryExplorerConstants.Attraction * (edge.Length() - StoryExplorerConstants.IdealLength);

                NodeForces[list.Edges[i].ThisIndex] -= attractionVec / list.Edges[i].This.Mass;
                NodeForces[list.Edges[i].ChildIndex] += attractionVec / list.Edges[i].Child.Mass;
            }

            //apply accelleration to nodes
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsPositionLocked)
                {
                    list[i].Position.X += NodeForces[i].X;
                    list[i].Position.Y += NodeForces[i].Y;
                }
            }
        }

        private void ResetNodeForces()
        {
            if (provider.OtherNodes.Count != NodeForces.Count)
            {
                NodeForces.Clear();
                for (int i = 0; i < provider.OtherNodes.Count; i++)
                {
                    NodeForces.Add(default);
                }
            }
            else
            {
                for (int i = 0; i < provider.OtherNodes.Count; i++)
                {
                    NodeForces[i] = default;
                }
            }
        }
    }
}
