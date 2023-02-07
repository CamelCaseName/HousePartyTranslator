using System.Collections.Generic;
using System.Xml.Linq;

namespace Translator.Explorer
{
    //a kind of an adjacencylist, but with edges and a direct node access in parallel
    internal class NodeList : List<Node>
    {
        //primarily used for rendering the edges, for graph stuff we use the actual multigraph in the nodes
        private readonly List<Edge> Edges = new();

        //todo get this working reliably, sync up all child/parent additions
        public new void Add(Node node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                Edges.Add(new(node, node.ChildNodes[i]));
            }
            for (int i = 0; i < node.ParentNodes.Count; i++)
            {
                Edges.Add(new(node.ParentNodes[i], node));
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
            for (int i = 0; i < node.ParentNodes.Count; i++)
            {
                Edges.Remove(new(node.ParentNodes[i], node));
            }
            return base.Remove(node);
        }

        //syncs edges to nodes
        public void Sync()
        {
            Edges.Clear();
            for (int x = 0; x < Count; x++)
            {
                for (int i = 0; i < this[x].ChildNodes.Count; i++)
                {
                    Edges.Add(new(this[x], this[x].ChildNodes[i]));
                }
                for (int i = 0; i < this[x].ParentNodes.Count; i++)
                {
                    Edges.Add(new(this[x].ParentNodes[i], this[x]));
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
                        Edges.Add(new(realList[i], realList[i].ChildNodes[c]));
                    }
                    for (int p = 0; p < realList[i].ParentNodes.Count; p++)
                    {
                        Edges.Add(new(realList[i].ParentNodes[p], realList[i]));
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
                        Edges.Add(new(en.Current, en.Current.ChildNodes[i]));
                    }
                    for (int i = 0; i < en.Current.ParentNodes.Count; i++)
                    {
                        Edges.Add(new(en.Current.ParentNodes[i], en.Current));
                    }
                }
            }
            base.AddRange(list);
        }

        public new void Clear()
        {
            base.Clear();
        }
    }
}
