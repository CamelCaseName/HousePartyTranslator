using System.Collections.Generic;
using System.Xml.Linq;

namespace Translator.Explorer
{
    //a kind of an adjacencylist, but with edges and a direct node access in parallel
    internal class NodeList : List<Node>
    {
        //primarily used for rendering the edges, for graph stuff we use the actual multigraph in the nodes
        public readonly List<Edge> Edges = new();

        //todo get this working reliably, sync up all child/parent additions
        public new void Add(Node node)
        {
            for (int c = 0; c < node.ChildNodes.Count; c++)
            {
                var edge = new Edge(node, node.ChildNodes[c]);
                if (!Edges.Contains(edge)) Edges.Add(edge);
            }
            base.Add(node);
        }

        public void AddChild(Node This, Node Child)
        {

        }

        public void AddParent(Node This, Node Child)
        {

        }

        public new bool Remove(Node node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                Edges.Remove(new(node, node.ChildNodes[i]));
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
                    var edge = new Edge(this[x], this[x].ChildNodes[c]);
                    if (!Edges.Contains(edge)) Edges.Add(edge);
                }
            }
        }

        public new void AddRange(IEnumerable<Node> list)
        {
            if (list is IList<Node> realList)
            {
                for (int i = 0; i < realList.Count; i++)
                {
                    for (int c = 0; c < realList[i].ChildNodes.Count; c++)
                    {
                        var edge = new Edge(realList[i], realList[i].ChildNodes[c]);
                        if (!Edges.Contains(edge)) Edges.Add(edge);
                    }
                }
            }
            else
            {
                using IEnumerator<Node> en = list.GetEnumerator();
                while (en.MoveNext())
                {
                    for (int i = 0; i < en.Current.ChildNodes.Count; i++)
                    {
                        var edge = new Edge(en.Current, en.Current.ChildNodes[i]);
                        if (!Edges.Contains(edge)) Edges.Add(edge);
                    }
                }
            }
            base.AddRange(list);
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
