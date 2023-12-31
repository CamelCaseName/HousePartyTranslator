using System;
using System.Windows.Forms;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Desktop.UI.Components
{
    public class WinToolStripTextBox : ToolStripTextBox, ITextBox
    {
        public int SelectionEnd
        {
            get => SelectionStart + SelectionLength;
            set
            {
                SelectionLength = SelectionStart < SelectionEnd
                ? value - SelectionStart
                : throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart");
            }
        }

        public int HighlightStart { get; set; }
        public int HighlightEnd { get; set; }
        public bool ShowHighlight { get; set; }
    }
}
