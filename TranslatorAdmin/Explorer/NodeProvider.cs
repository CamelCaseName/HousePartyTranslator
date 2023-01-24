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
	}
}
