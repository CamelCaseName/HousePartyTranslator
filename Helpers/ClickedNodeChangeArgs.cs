using System;

namespace HousePartyTranslator.Helpers
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
