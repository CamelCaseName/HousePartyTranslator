using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinToolStripTextBox : ToolStripTextBox, ITextBox
    {
        public int SelectionEnd
        {
            get => base.SelectionStart + SelectionLength;
            set { if (SelectionStart < SelectionEnd) SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
        }
    }
}
