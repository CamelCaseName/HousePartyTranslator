using System;

namespace Translator.Desktop.Explorer.Graph
{
    internal sealed class ClickedNodeChangeArgs : EventArgs
    {
        public Node ChangedNode { get; }

        public ClickedNodeTypes ClickType { get; }

        public ClickedNodeChangeArgs(Node highlightNode, ClickedNodeTypes highlightCase)
        {
            ChangedNode = highlightNode;
            ClickType = highlightCase;
        }
    }
}
