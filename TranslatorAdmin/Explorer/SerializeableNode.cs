using static Translator.Explorer.JSON.StoryEnums;

namespace Translator.Explorer
{
	internal sealed class SerializeableNode
	{
		public PointF Position;
		public string? ID;
		public string? Text;
		public NodeType Type;
		public int Mass = 1;
		public Gender Gender = 0;
		public bool Visited = false;
		public bool ChildsVisited = false;
		public bool ParentsVisited = false;
		public Guid Guid = Guid.NewGuid();
		public Guid[]? ParentNodes;
		public Guid[]? ChildNodes;
		public string FileName = string.Empty;
		public bool IsPositionLocked;

		public static explicit operator SerializeableNode(Node v)
		{
			var node = new SerializeableNode
			{
				ChildsVisited = v.ChildsVisited,
				Guid = v.Guid,
				ID = v.ID,
				Mass = v.Mass,
				ParentsVisited = v.ParentsVisited,
				Position = v.Position,
				Gender = v.Gender,
				Text = v.Text,
				Type = v.Type,
				Visited = v.Visited,
				FileName = v.FileName,
				ChildNodes = new Guid[v.ChildNodes.Count],
				ParentNodes = new Guid[v.ParentNodes.Count],
				IsPositionLocked = v.IsPositionLocked,
			};

			//add missing nodes as guid references
			if (v.ChildNodes.Count > 0)
			{
				int i = 0;
				foreach (Node child in v.ChildNodes)
				{
					node.ChildNodes[i++] = child.Guid;
				}
			}

			//also add parent nodes as guid references for later recosntruction
			if (v.ParentNodes.Count > 0)
            {
                int i = 0;
                foreach (Node parent in v.ParentNodes)
                {
                    node.ParentNodes[i++] = parent.Guid;
                }
            }

			return node;
		}
	}
}
