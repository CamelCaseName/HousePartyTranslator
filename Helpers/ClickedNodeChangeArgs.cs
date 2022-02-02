using System;

namespace HousePartyTranslator.Helpers
{
    public class ClickedNodeChangeArgs : EventArgs
    {
        public Node HighlightNode { get; }

        public ClickedNodeTypes HighlightCase { get; }

        public ClickedNodeChangeArgs(Node highlightNode, ClickedNodeTypes highlightCase)
        {
            HighlightNode = highlightNode;
            HighlightCase = highlightCase;
        }
    }
}
