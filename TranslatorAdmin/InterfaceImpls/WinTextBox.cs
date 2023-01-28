using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;

namespace TranslatorApp.InterfaceImpls
{
	internal class WinTextBox : TextBox, ITextBox
	{
		public int SelectionEnd
		{
			get => base.SelectionStart + SelectionLength;
			set { if (SelectionStart <= SelectionEnd) SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
		}
		public new int SelectionStart { get => base.SelectionStart; set => base.SelectionStart = value; }
		public int HighlightStart { get; set; }
		public int HighlightEnd { get; set; }
		public bool ShowHighlight { get; set; }
		public new string Text { get => base.Text; set => base.Text = value; }

		public new void Focus() => base.Focus();

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!ShowHighlight || HighlightEnd <= HighlightStart || HighlightStart >= Text.Length || HighlightEnd >= Text.Length)
				base.OnPaint(e);
			else
			{
				base.OnPaint(e);
				//overlay the other text highlight
				var startSize = TextRenderer.MeasureText(Text.AsSpan()[..(HighlightStart - 1)], Font, ClientRectangle.Size);
				var endSize = TextRenderer.MeasureText(Text.AsSpan()[..(HighlightEnd)], Font, ClientRectangle.Size);
				Point newPosition = (Point)(endSize - startSize);
				TextRenderer.DrawText(e.Graphics, Text.AsSpan()[HighlightStart..HighlightEnd], Font, newPosition, Utils.brightText, Utils.highlight);
			}
		}
	}
}
