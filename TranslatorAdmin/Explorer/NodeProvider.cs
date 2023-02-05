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
        private List<Node> InternalNodes = new();
        private readonly List<Node> nodesA = new();
        private readonly List<Node> nodesB = new();
        public bool UsingListA = true;
        private bool frozen = false;
        private (int lockedNodeIndex, float movedNodePos_X, float movedNodePos_Y) _movingNodeInfo = (-1, 0.0f, 0.0f);

        public (int lockedNodeIndex, float movedNodePos_X, float movedNodePos_Y) MovingNodeInfo => _movingNodeInfo;

        public List<Node> Nodes
        {
            get { CheckNodeListSizes(); return UsingListA ? nodesA : nodesB; }
        }

        //only to be used in calcualation, dont set or youll break things
        public List<Node> OtherNodes
        {
            get { CheckNodeListSizes(); return !UsingListA ? nodesA : nodesB; }
        }

        public bool MovingNodePositionOverridden
        {
            get;
            private set;
        }
        public bool MovingNodePositionOverrideEnded { get; private set; }

        private void CheckNodeListSizes()
        {
            if (nodesA.Count != nodesB.Count)
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
