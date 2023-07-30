using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;

namespace Translator.Desktop.Explorer.Graph
{

    //# acts as the intermediary between display and calculation,
    // * keeps track of all nodes
    // * allows the addition and removal of nodes (tbd)
    // * allows to show only the nodes defined by rules (tbd)
    // * allows to only calulate the positions for defined nodes (tbd)
    // * allows to save all nodes back into a story (tbd)
    [SupportedOSPlatform("windows")]
    internal sealed class NodeProvider
    {
        private NodeList InternalNodes = new();
        private readonly NodeList nodesA = new();
        private readonly NodeList nodesB = new();
        private bool _usingListA = true;
        private readonly List<NodeType> allowedTypes = new();
        public bool UsingListA
        {
            get
            {
                return !frozen || _usingListA;
            }
            set
            {
                if (frozen)
                    _usingListA = value;
            }
        }
        private bool frozen = false;
        private NodeMovementInfo _movingNodeInfo = (-1, 0.0f, 0.0f);
        public NodeMovementInfo MovingNodeInfo => _movingNodeInfo;

        public NodeList Nodes
        {
            get { CheckNodeListSizes(); return UsingListA ? nodesA : nodesB; }
        }

        //only to be used in calcualation, dont set or youll break things
        public NodeList OtherNodes
        {
            get { CheckNodeListSizes(); return !UsingListA ? nodesA : nodesB; }
        }

        public bool MovingNodePositionOverridden { get; private set; }

        public bool MovingNodePositionOverrideEnded { get; private set; }

        public bool Frozen { get => frozen; }

        private void CheckNodeListSizes()
        {
            if (nodesA.Count == nodesB.Count && nodesA.Edges.Count == nodesB.Edges.Count) return;
            if (!frozen) return;

            if (nodesA.Count != nodesB.Count)
            {
                //only recalculate when necessary 
                if (nodesA.Edges.Count != nodesB.Edges.Count)
                {
                    lock (nodesB.Edges)
                        lock (nodesA.Edges)
                        {
                            if (UsingListA)
                            {
                                //changed edges is in a
                                nodesA.Sync();
                            }
                            else
                            {
                                //changed edges is in b, will be copied later
                                nodesB.Sync();
                            }
                        }
                }

                lock (nodesB)
                    lock (nodesA)
                    {
                        MovingNodePositionOverridden = true;
                        if (UsingListA)
                        {
                            //changed amount is in a
                            nodesB.Clear();
                            nodesB.AddRange(nodesA);
                        }
                        else
                        {
                            //changed amount is in b
                            nodesA.Clear();
                            nodesA.AddRange(nodesB);
                        }
                    }


            }

            if (nodesA.Edges.Count == nodesB.Edges.Count) return;

            lock (nodesB.Edges)
                lock (nodesA.Edges)
                {
                    if (UsingListA)
                    {
                        //changed edges is in a
                        if (Math.Abs(nodesA.Edges.Count - nodesB.Edges.Count) > 1) nodesA.Sync();
                        nodesB.Edges.Clear();
                        nodesB.Edges.AddRange(nodesA.Edges);
                    }
                    else
                    {
                        //changed edges is in b
                        if (Math.Abs(nodesA.Edges.Count - nodesB.Edges.Count) > 1) nodesA.Sync();
                        nodesA.Edges.Clear();
                        nodesA.Edges.AddRange(nodesB.Edges);
                    }
                }
        }

        public NodeProvider()
        {
        }

        public void FreezeNodesAsInitial()
        {
            if (!frozen)
            {
                Nodes.Sync();//sync once before we save them finally
                InternalNodes = Nodes;
                nodesB.Clear();
                nodesB.AddRange(nodesA);
                frozen = true;
            }
        }

        internal void ConsumedNodePositionOverrideEnded()
        {
            MovingNodePositionOverridden = false;
            MovingNodePositionOverrideEnded = false;
            _movingNodeInfo = (-1, 0.0f, 0.0f);
        }

        internal void SignalPositionChangeBegin(int index)
        {
            if (index >= 0)
            {
                _movingNodeInfo.Index = index;
                _movingNodeInfo.PosX = Nodes[index].Position.X;
                _movingNodeInfo.PosY = Nodes[index].Position.Y;
                MovingNodePositionOverrideEnded = false;
                MovingNodePositionOverridden = true;
            }
        }

        internal void UpdatePositionChange(float x, float y)
        {
            _movingNodeInfo.PosX = x;
            _movingNodeInfo.PosY = y;
        }

        internal void EndPositionChange()
        {
            MovingNodePositionOverrideEnded = true;
        }

        internal List<PointF> GetPositions()
        {
            return InternalNodes.GetPositions();
        }

        internal List<PointF> GetVolatilePositions()
        {
            return Nodes.GetPositions();
        }

        internal void AddFilter(NodeType allowedType)
        {
            if (!allowedTypes.Contains(allowedType))
                allowedTypes.Add(allowedType);
        }

        internal void RemoveFilter(NodeType disallowedType)
        {
            allowedTypes.Remove(disallowedType);
        }
    }
}
