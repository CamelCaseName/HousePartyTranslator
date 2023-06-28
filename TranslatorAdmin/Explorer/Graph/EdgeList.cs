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
            if (base.Add(e))
            {
                _edges.Add(e);
                return true;
            }
            return false;
        }

        public new bool Remove(Edge e)
        {
            if (base.Remove(e))
            {
                _edges.Remove(e);
                return true;
            }
            return false;
        }

        public void AddRange(IEnumerable<Edge> edgesToAdd)
        {
            var enumerator = edgesToAdd.GetEnumerator();
            while (enumerator.MoveNext())
            {
                base.Add(enumerator.Current);
            }
            _edges.AddRange(edgesToAdd);
        }

        public new void Clear()
        {
            _edges.Clear();
            base.Clear();
        }
    }
}
