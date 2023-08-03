using System.Collections.Generic;

namespace Translator.Desktop.Explorer.Graph
{
    internal sealed class EdgeList : HashSet<Edge>
    {
        private readonly List<Edge> _edges = new();

        public Edge this[int index]
        {
            get
            {
                return _edges[index];
            }
        }

        public new bool Add(Edge e)
        {
            lock (this)
                lock (_edges)
                {
                    if (base.Add(e))
                    {
                        _edges.Add(e);
                        return true;
                    }
                    return false;
                }
        }

        public new bool Remove(Edge e)
        {
            lock (this)
                lock (_edges)
                {
                    if (base.Remove(e))
                    {
                        _edges.Remove(e);
                        return true;
                    }
                    return false;
                }
        }

        public void AddRange(IEnumerable<Edge> edgesToAdd)
        {
            lock (this)
                lock (_edges)
                    lock (edgesToAdd)
                    {
                        IEnumerator<Edge> enumerator = edgesToAdd.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            base.Add(enumerator.Current);
                        }
                        _edges.AddRange(edgesToAdd);
                    }
        }

        public new void Clear()
        {
            lock (this)
                lock (_edges)
                {
                    _edges.Clear();
                    base.Clear();
                }
        }
    }
}
