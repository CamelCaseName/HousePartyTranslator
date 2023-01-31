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

		public List<Node> Nodes
		{
			get { CheckNodeListSizes(); return UsingListA ? nodesA : nodesB; }
		}
		//only to be used in calcualation, dont set or youll break things
		public List<Node> OtherNodes
		{
			get { CheckNodeListSizes(); return !UsingListA ? nodesA : nodesB; }
		}

		private void CheckNodeListSizes()
		{
			if (nodesA.Count != nodesB.Count)
			{
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
			if (positionBuffer.Length != Nodes.Count * 4)
				positionBuffer = new float[Nodes.Count * 4];
			for (int i = 0; i < Nodes.Count; i++)
			{
				positionBuffer[i * 4] = Nodes[i].Position.X;
				positionBuffer[(i * 4 + 1)] = Nodes[i].Position.Y;
				positionBuffer[(i * 4 + 2)] = Nodes[i].IsPositionLocked ? 1.0f : 0.0f;
				positionBuffer[(i * 4 + 3)] = Nodes[i].Mass;
			}
			return positionBuffer;
		}

		public float[] GetNodeNewPositionBuffer()
		{
			if (returnedPositionBuffer.Length != Nodes.Count * 4)
				returnedPositionBuffer = new float[Nodes.Count * 4];
			for (int i = 0; i < Nodes.Count; i++)
			{
				returnedPositionBuffer[i * 4] = Nodes[i].Position.X;
				returnedPositionBuffer[(i * 4 + 1)] = Nodes[i].Position.Y;
				returnedPositionBuffer[(i * 4 + 2)] = Nodes[i].IsPositionLocked ? 1.0f : 0.0f;
				returnedPositionBuffer[(i * 4 + 3)] = Nodes[i].Mass;
			}
			return returnedPositionBuffer;
		}

		public (int[] indices, int[] offsets, int[] count) GetNodeParentsBuffer()
		{
			int offset = 0;
			parent_count.Clear();
			parent_indices.Clear();
			parent_offset.Clear();
			for (int i = 0; i < Nodes.Count; i++)
			{
				//from offset
				parent_offset.Add(offset);
				for (int j = 0; j < Nodes[i].ParentNodes.Count; j++)
				{
					int index = Nodes.IndexOf(Nodes[i].ParentNodes[j]);
					if (index == -1) continue;

					parent_indices.Add(index);
					offset++;
				}
				//till count are our edges
				parent_count.Add(Nodes[i].ParentNodes.Count);
			}

			return (parent_indices.ToArray(), parent_offset.ToArray(), parent_count.ToArray());
		}

		internal (int[] node_childs, int[] node_childs_offset, int[] node_childs_count) GetNodeChildsBuffer()
		{
			int offset = 0;
			child_count.Clear();
			child_indices.Clear();
			child_offset.Clear();
			for (int i = 0; i < Nodes.Count; i++)
			{
				child_offset.Add(offset);
				for (int j = 0; j < Nodes[i].ChildNodes.Count; j++)
				{
					int index = Nodes.IndexOf(Nodes[i].ChildNodes[j]);
					if (index == -1) continue;

					child_indices.Add(index);
					offset++;
				}
				child_count.Add(Nodes[i].ChildNodes.Count);
			}

			return (child_indices.ToArray(), child_offset.ToArray(), child_count.ToArray());
		}

		internal void SetNewNodePositions(float[] returnedNodePositionBuffer)
		{
			for (int i = 0; i < returnedNodePositionBuffer.Length / 4; i++)
			{
				OtherNodes[i].Position.X = returnedNodePositionBuffer[i * 4];
				OtherNodes[i].Position.Y = returnedNodePositionBuffer[(i * 4) + 1];
				OtherNodes[i].Mass = (int)returnedNodePositionBuffer[(i * 4) + 3];
			}
		}
	}
}
