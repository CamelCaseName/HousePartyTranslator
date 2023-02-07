using Translator.UICompatibilityLayer;

namespace Translator.InterfaceImpls
{
	internal class WinToolStripTextBox : ToolStripTextBox, ITextBox
	{
		public int SelectionEnd
		{
			get => base.SelectionStart + SelectionLength;
			set { if (SelectionStart < SelectionEnd) SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
		}

		public int HighlightStart { get; set; }
		public int HighlightEnd { get; set; }
		public bool ShowHighlight { get; set; }
	}
}
