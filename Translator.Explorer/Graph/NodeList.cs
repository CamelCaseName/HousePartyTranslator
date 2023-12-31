using System;
using System.Collections.Generic;
using System.Drawing;
using Translator.Core.Helpers;

namespace Translator.Explorer.Graph
{
    public sealed class NodeCountChangedArgs : EventArgs
    {
        public int Count;
        public NodeCountChangedArgs(int size) { Count = size; }
    }

    public sealed class EdgeCountChangedArgs : EventArgs
    {
        public int Count;
        public EdgeCountChangedArgs(int size) { Count = size; }
    }

    //a kind of an adjacencylist, but with edges and a direct node access in parallel
    public sealed class NodeList : List<Node>
    {
        //primarily used for rendering the edges, for graph stuff we use the actual multigraph in the nodes
        public readonly EdgeList Edges = new();
        public readonly Dictionary<NodeType, int> Types = new();

        public event EventHandler<NodeCountChangedArgs>? NodeCountChanged;
        public event EventHandler<EdgeCountChangedArgs>? EdgeCountChanged;

        public new void Add(Node node)
        {
            lock (this)
                lock (Edges)
                    lock (Types)
                    {
                        base.Add(node);
                        for (int c = 0; c < node.ChildNodes.Count; c++)
                        {
                            var edge = new Edge(node, Count - 1, node.ChildNodes[c], IndexOf(node.ChildNodes[c]));
                            Edges.Add(edge);
                        }

                        if (!Types.ContainsKey(node.Type))
                            Types.Add(node.Type, 1);
                        else
                            Types[node.Type] = Types[node.Type] + 1;

                        if (NodeCountChanged is not null)
                            NodeCountChanged(this, new(Count));
                        if (EdgeCountChanged is not null)
                            EdgeCountChanged(this, new(Edges.Count));
                    }
        }

        public new bool Remove(Node node)
        {
            lock (this)
                lock (Edges)
                    lock (Types)
                    {
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                            Edges.Remove(new(node, 0, node.ChildNodes[i], 0));

                        bool res = base.Remove(node);

                        if (Types[node.Type] == 1)
                            Types.Remove(node.Type);
                        else
                            Types[node.Type] = Types[node.Type] - 1;

                        if (NodeCountChanged is not null)
                            NodeCountChanged(this, new(Count));
                        if (EdgeCountChanged is not null)
                            EdgeCountChanged(this, new(Edges.Count));

                        return res;
                    }
        }

        //syncs edges to nodes, including nodes not in the list
        public void Sync()
        {
#if DEBUG
            var start = DateTime.UtcNow;
#endif
            lock (this)
                lock (Edges)
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
#if DEBUG
            LogManager.Log($"Sync on {Count} nodes, {Edges.Count} edges took {(DateTime.UtcNow - start).Milliseconds}ms");
#endif
        }

        //syncs edges to nodes, but exludes all edges that would include nodes not in the list
        public void StrictSync()
        {
#if DEBUG
            var start = DateTime.UtcNow;
#endif
            lock (this)
                lock (Edges)
                {
                    Edges.Clear();
                    for (int x = 0; x < Count; x++)
                    {
                        for (int c = 0; c < this[x].ChildNodes.Count; c++)
                        {
                            TryCreateEdge(this[x], this[x].ChildNodes[c], x);
                        }
                    }
                    if (EdgeCountChanged is not null)
                        EdgeCountChanged(this, new(Edges.Count));
                }
#if DEBUG
            LogManager.Log($"StrictSync on {Count} nodes, {Edges.Count} edges took {(DateTime.UtcNow - start).Milliseconds}ms");
#endif
        }

        private void TryCreateEdge(Node root, Node child, int x, int depth = 3)
        {
            //values for depth over 3 break the thing, so this has to suffice
            //to stop infinite cycles that might arise, but should not
            if (depth == 0 || root == child) return;

            //Add edge if we have the node as is
            int c = IndexOf(child);
            if (c >= 0)
                Edges.Add(new Edge(root, x, child, c));
            else
            {
                //try and create an edge to the childs of the child if possible
                foreach (var childsChild in child.ChildNodes)
                {
                    TryCreateEdge(root, childsChild, x, depth - 1);
                }
            }
        }

        public new void AddRange(IEnumerable<Node> list)
        {
            lock (this)
                lock (Edges)
                    lock (Types)
                        lock (list)
                        {
                            base.AddRange(list);
                            if (list is NodeList realList)
                            {
                                foreach (KeyValuePair<NodeType, int> kvp in realList.Types)
                                {
                                    if (!Types.ContainsKey(kvp.Key))
                                        Types.Add(kvp.Key, kvp.Value);
                                    else
                                        Types[kvp.Key] = Types[kvp.Key] + kvp.Value;
                                }

                                lock (realList.Edges)
                                    Edges.AddRange(realList.Edges);
                            }
                            else
                            {
                                using IEnumerator<Node> en = list.GetEnumerator();
                                while (en.MoveNext())
                                {
                                    if (!Types.ContainsKey(en.Current.Type))
                                        Types.Add(en.Current.Type, 1);
                                    else
                                        Types[en.Current.Type] = Types[en.Current.Type] + 1;

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
        }

        public new void Clear()
        {
            lock (this)
                lock (Edges)
                {
                    Edges.Clear();
                    base.Clear();
                    Types.Clear();
                    if (NodeCountChanged is not null)
                        NodeCountChanged(this, new(Count));
                    if (EdgeCountChanged is not null)
                        EdgeCountChanged(this, new(Edges.Count));
                }
        }

        public List<PointF> GetPositions()
        {
            List<PointF> positions = new(Count);
            for (int i = 0; i < Count; i++)
            {
                positions.Add(this[i].Position);
            }
            return positions;
        }

        public bool SetPositions(List<PointF> positions)
        {
            if (positions.Count != Count) return false;
            for (int i = 0; i < Count; i++)
            {
                this[i].Position = positions[i];
            }
            return true;
        }

        public new Node this[int index]
        {
            get
            {
                if (Count > 0 && index >= 0) return base[index];
                else return Node.NullNode;
            }
            set
            {
                base[index] = value;
            }
        }
    }
}
