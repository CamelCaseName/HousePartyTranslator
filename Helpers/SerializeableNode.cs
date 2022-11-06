using System;
using System.Collections.Generic;
using System.Drawing;

namespace HousePartyTranslator.Helpers
{
    internal sealed class SerializeableNode
    {
        public Point Position;
        public string ID;
        public string Text;
        public NodeType Type;
        public int Mass = 1;
        public Gender Gender = 0;
        public bool Visited = false;
        public bool ChildsVisited = false;
        public bool ParentsVisited = false;
        public Guid Guid = Guid.NewGuid();
        public List<Guid> ParentNodes;
        public List<Guid> ChildNodes;

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
                ChildNodes = new List<Guid>(),
                ParentNodes = new List<Guid>()
            };

            //add missing nodes as guid references
            if (v.ChildNodes.Count > 0)
            {
                foreach (Node child in v.ChildNodes)
                {
                    node.ChildNodes.Add(child.Guid);
                }
            }

            //also add parent nodes as guid references for later recosntruction
            if (v.ParentNodes.Count > 0)
            {
                foreach (Node parent in v.ParentNodes)
                {
                    node.ParentNodes.Add(parent.Guid);
                }
            }

            return node;
        }
    }
}
