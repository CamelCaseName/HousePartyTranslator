using System.Runtime.Versioning;

namespace Translator.Explorer
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
        public bool UsingListA = true;
        private bool frozen = false;
        private (int lockedNodeIndex, float movedNodePos_X, float movedNodePos_Y) _movingNodeInfo = (-1, 0.0f, 0.0f);

        public (int lockedNodeIndex, float movedNodePos_X, float movedNodePos_Y) MovingNodeInfo => _movingNodeInfo;

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
            if (nodesA.Count != nodesB.Count)
            {
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


                //only recalculate when necessary 
                if (nodesA.Edges.Count != nodesB.Edges.Count && frozen)
                {
                    lock (nodesB.Edges)
                        lock (nodesA.Edges)
                        {
                            if (UsingListA)
                            {
                                //changed edges is in a
                                nodesA.Sync();
                                nodesB.Edges.Clear();
                                nodesB.Edges.AddRange(nodesA.Edges);
                            }
                            else
                            {
                                //changed edges is in b
                                nodesB.Sync();
                                nodesA.Edges.Clear();
                                nodesA.Edges.AddRange(nodesB.Edges);
                            }
                        }
                }
            }

            if (nodesA.Edges.Count != nodesB.Edges.Count && frozen)
            {
                lock (nodesB.Edges)
                    lock (nodesA.Edges)
                    {
                        if (UsingListA)
                        {
                            //changed edges is in a
                            nodesB.Edges.Clear();
                            nodesB.Edges.AddRange(nodesA.Edges);
                        }
                        else
                        {
                            //changed edges is in b
                            nodesA.Edges.Clear();
                            nodesA.Edges.AddRange(nodesB.Edges);
                        }
                    }
            }
        }

        public NodeProvider()
        {
        }

        public void FreezeNodesAsInitial()
        {
            if (!frozen) InternalNodes = Nodes; frozen = true;
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
                _movingNodeInfo.lockedNodeIndex = index;
                _movingNodeInfo.movedNodePos_X = Nodes[index].Position.X;
                _movingNodeInfo.movedNodePos_Y = Nodes[index].Position.Y;
                MovingNodePositionOverrideEnded = false;
                MovingNodePositionOverridden = true;
            }
        }

        internal void UpdatePositionChange(float x, float y)
        {
            _movingNodeInfo.movedNodePos_X = x;
            _movingNodeInfo.movedNodePos_Y = y;
        }

        internal void EndPositionChange()
        {
            MovingNodePositionOverrideEnded = true;
        }
    }
}
