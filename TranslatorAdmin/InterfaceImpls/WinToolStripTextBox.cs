using Translator.Core.UICompatibilityLayer;

namespace Translator.Desktop.InterfaceImpls
{
    internal class WinToolStripTextBox : ToolStripTextBox, ITextBox
    {
        public int SelectionEnd
        {
            get => SelectionStart + SelectionLength;
            set { if (SelectionStart < SelectionEnd) SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
        }

        public int HighlightStart { get; set; }
        public int HighlightEnd { get; set; }
        public bool ShowHighlight { get; set; }
    }
}
