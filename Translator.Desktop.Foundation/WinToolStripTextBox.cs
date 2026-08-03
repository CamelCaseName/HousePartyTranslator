using System;
using System.Windows.Forms;
using Translator.Core.UICompatibilityLayer;
using System.ComponentModel;

namespace Translator.Desktop.UI.Components
{
    public class WinToolStripTextBox : ToolStripTextBox, ITextBox
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HighlightStart { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HighlightEnd { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowHighlight { get; set; }
    }
}
