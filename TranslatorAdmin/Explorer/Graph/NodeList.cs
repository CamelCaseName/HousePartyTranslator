using System.Collections.Generic;
using System.Drawing;

namespace Translator.Desktop.Explorer.Graph
{
    //a kind of an adjacencylist, but with edges and a direct node access in parallel
    internal class NodeList : List<Node>
    {
        //primarily used for rendering the edges, for graph stuff we use the actual multigraph in the nodes
        public readonly List<Edge> Edges = new();

        public new void Add(Node node)
        {
            base.Add(node);
            for (int c = 0; c < node.ChildNodes.Count; c++)
            {
                var edge = new Edge(node, Count - 1, node.ChildNodes[c], IndexOf(node.ChildNodes[c]));
                if (!Edges.Contains(edge)) Edges.Add(edge);
            }
        }

        public new bool Remove(Node node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                Edges.Remove(new(node, 0, node.ChildNodes[i], 0));
            }
            return base.Remove(node);
        }

        //syncs edges to nodes
        public void Sync()
        {
            Edges.Clear();
            for (int x = 0; x < Count; x++)
            {
                for (int c = 0; c < this[x].ChildNodes.Count; c++)
                {
                    var edge = new Edge(this[x], x, this[x].ChildNodes[c], IndexOf(this[x].ChildNodes[c]));
                    if (!Edges.Contains(edge)) Edges.Add(edge);
                }
            }
        }

        public new void AddRange(IEnumerable<Node> list)
        {
            base.AddRange(list);
            if (list is NodeList realList)
            {
                Edges.AddRange(realList.Edges);
            }
            else
            {
                using IEnumerator<Node> en = list.GetEnumerator();
                while (en.MoveNext())
                {
                    for (int i = 0; i < en.Current.ChildNodes.Count; i++)
                    {
                        var edge = new Edge(en.Current, IndexOf(en.Current), en.Current.ChildNodes[i], IndexOf(en.Current.ChildNodes[i]));
                        if (!Edges.Contains(edge)) Edges.Add(edge);
                    }
                }
            }
        }

        public new void Clear()
        {
            base.Clear();
        }

        internal List<PointF> GetPositions()
        {
            List<PointF> positions = new(Count);
            for (int i = 0; i < Count; i++)
            {
                positions.Add(this[i].Position);
            }
            return positions;
        }

        internal bool SetPositions(List<PointF> positions)
        {
            if (positions.Count != Count) return false;
            for (int i = 0; i < Count; i++)
            {
                this[i].Position = positions[i];
            }
            return true;
        }
    }
}
