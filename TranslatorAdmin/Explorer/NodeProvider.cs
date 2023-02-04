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
        private float[] positionBuffer = Array.Empty<float>();
        private float[] returnedPositionBuffer = Array.Empty<float>();
        private readonly List<int> parent_indices = new();
        private readonly List<int> parent_offset = new();
        private readonly List<int> parent_count = new();
        private readonly List<int> child_indices = new();
        private readonly List<int> child_offset = new();
        private readonly List<int> child_count = new();
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

        public float[] GetNodePositionBuffer()
        {
            //cache the array so we do less allocations
            if (positionBuffer.Length != OtherNodes.Count * 4)
                positionBuffer = new float[OtherNodes.Count * 4];

            //set new data
            for (int i = 0; i < OtherNodes.Count; i++)
            {
                positionBuffer[i * 4] = OtherNodes[i].Position.X;
                positionBuffer[(i * 4) + 1] = OtherNodes[i].Position.Y;
                positionBuffer[(i * 4) + 2] = OtherNodes[i].IsPositionLocked ? 1.0f : 0.0f;
                positionBuffer[(i * 4) + 3] = OtherNodes[i].Mass;
            }
            return positionBuffer;
        }

        public float[] GetNodeNewPositionBuffer()
        {
            if (returnedPositionBuffer.Length != OtherNodes.Count * 4)
                returnedPositionBuffer = new float[OtherNodes.Count * 4];
            return returnedPositionBuffer;
        }

        public (int[] indices, int[] offsets, int[] count) GetNodeParentsBuffer()
        {
            int offset = 0;
            parent_count.Clear();
            parent_indices.Clear();
            parent_offset.Clear();
            for (int i = 0; i < OtherNodes.Count; i++)
            {
                //from offset
                parent_offset.Add(offset);
                for (int j = 0; j < OtherNodes[i].ParentNodes.Count; j++)
                {
                    int index = OtherNodes.IndexOf(OtherNodes[i].ParentNodes[j]);
                    if (index == -1) continue;

                    parent_indices.Add(index);
                    offset++;
                }
                //till count are our edges
                parent_count.Add(OtherNodes[i].ParentNodes.Count);
            }

            return (parent_indices.ToArray(), parent_offset.ToArray(), parent_count.ToArray());
        }

        internal (int[] node_childs, int[] node_childs_offset, int[] node_childs_count) GetNodeChildsBuffer()
        {
            int offset = 0;
            child_count.Clear();
            child_indices.Clear();
            child_offset.Clear();
            for (int i = 0; i < OtherNodes.Count; i++)
            {
                child_offset.Add(offset);
                for (int j = 0; j < OtherNodes[i].ChildNodes.Count; j++)
                {
                    int index = OtherNodes.IndexOf(OtherNodes[i].ChildNodes[j]);
                    if (index == -1) continue;

                    child_indices.Add(index);
                    offset++;
                }
                child_count.Add(OtherNodes[i].ChildNodes.Count);
            }

            return (child_indices.ToArray(), child_offset.ToArray(), child_count.ToArray());
        }

        internal void SetNewNodePositions(float[] returnedNodePositionBuffer)
        {
            for (int i = 0; i < returnedNodePositionBuffer.Length / 4; i++)
            {
                if (returnedNodePositionBuffer[(i * 4) + 2] == 0.0f)
                {
                    OtherNodes[i].Position.X = returnedNodePositionBuffer[i * 4];
                    OtherNodes[i].Position.Y = returnedNodePositionBuffer[(i * 4) + 1];
                }
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
