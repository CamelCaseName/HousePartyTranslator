﻿using Translator.UICompatibilityLayer;

namespace TranslatorApp.InterfaceImpls
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
