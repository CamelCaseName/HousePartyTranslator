using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;

namespace Translator.Desktop.Explorer.Graph
{
    internal sealed class NodeCountChangedArgs : EventArgs
    {
        public int Count;
        public NodeCountChangedArgs(int size) { Count = size; }
    }
    internal sealed class EdgeCountChangedArgs : EventArgs
    {
        public int Count;
        public EdgeCountChangedArgs(int size) { Count = size; }
    }

    //a kind of an adjacencylist, but with edges and a direct node access in parallel
    internal sealed class NodeList : List<Node>
    {
        //primarily used for rendering the edges, for graph stuff we use the actual multigraph in the nodes
        public readonly EdgeList Edges = new();

        public event EventHandler<NodeCountChangedArgs>? NodeCountChanged;
        public event EventHandler<EdgeCountChangedArgs>? EdgeCountChanged;

        public new void Add(Node node)
        {
            base.Add(node);
            for (int c = 0; c < node.ChildNodes.Count; c++)
            {
                var edge = new Edge(node, Count - 1, node.ChildNodes[c], IndexOf(node.ChildNodes[c]));
                Edges.Add(edge);
            }
            if (NodeCountChanged is not null)
                NodeCountChanged(this, new(Count));
            if (EdgeCountChanged is not null)
                EdgeCountChanged(this, new(Edges.Count));
        }

        public new bool Remove(Node node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                Edges.Remove(new(node, 0, node.ChildNodes[i], 0));
            }
            bool res = base.Remove(node);
            if (NodeCountChanged is not null)
                NodeCountChanged(this, new(Count));
            if (EdgeCountChanged is not null)
                EdgeCountChanged(this, new(Edges.Count));
            return res;
        }

        //syncs edges to nodes, including nodes not in the list
        public void Sync()
        {
            Edges.Clear();
            for (int x = 0; x < Count; x++)
            {
                for (int c = 0; c < this[x].ChildNodes.Count; c++)
                {
                    var edge = new Edge(this[x], x, this[x].ChildNodes[c], IndexOf(this[x].ChildNodes[c]));
                    Edges.Add(edge);
                }
            }
            if (EdgeCountChanged is not null)
                EdgeCountChanged(this, new(Edges.Count));
        }

        //syncs edges to nodes, but exludes all edges that would include nodes not in the list
        public void StrictSync()
        {
            Edges.Clear();
            for (int x = 0; x < Count; x++)
            {
                for (int c = 0; c < this[x].ChildNodes.Count; c++)
                {
                    if (Contains(this[x].ChildNodes[c]))
                    {
                        var edge = new Edge(this[x], x, this[x].ChildNodes[c], IndexOf(this[x].ChildNodes[c]));
                        Edges.Add(edge);
                    }
                }
            }
            if (EdgeCountChanged is not null)
                EdgeCountChanged(this, new(Edges.Count));
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
                        Edges.Add(edge);
                    }
                }
            }
            if (NodeCountChanged is not null)
                NodeCountChanged(this, new(Count));
            if (EdgeCountChanged is not null)
                EdgeCountChanged(this, new(Edges.Count));
        }

        public new void Clear()
        {
            Edges.Clear();
            base.Clear();
            if (NodeCountChanged is not null)
                NodeCountChanged(this, new(Count));
            if (EdgeCountChanged is not null)
                EdgeCountChanged(this, new(Edges.Count));
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
