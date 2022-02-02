using System;

namespace HousePartyTranslator.Helpers
{
    public class ClickedNodeChangeArgs : EventArgs
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
