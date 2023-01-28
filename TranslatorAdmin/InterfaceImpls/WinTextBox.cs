using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.Versioning;
using System.Text;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;

namespace TranslatorApp.InterfaceImpls
{
	[SupportedOSPlatform("windows")]
	public class WinTextBox : TextBox, ITextBox
	{

		public WinTextBox() : base()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.FixedHeight, false);
		}
		public int SelectionEnd
		{
			get => base.SelectionStart + base.SelectionLength;
			set { if (SelectionStart <= SelectionEnd) base.SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
		}
		public new int SelectionStart { get => base.SelectionStart; set => base.SelectionStart = value; }
		public int HighlightStart { get; set; }
		public int HighlightEnd { get; set; }
		public bool ShowHighlight { get; set; }
		public new string Text { get => base.Text; set => base.Text = value; }

		public new void Focus() => base.Focus();

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			string adjustedText = Text;
			e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
			//do word wrap
			int maxlineLength = 0;
			if (WordWrap && Text.Length > 0)
			{
				while (++maxlineLength < Text.Length)
				{
					var size = e.Graphics.MeasureString(Text[..maxlineLength], base.Font);
					if (size.Width > ClientRectangle.Width)
					{
						adjustedText = Text.ConstrainLength(maxlineLength + 1);
						break;
					}
				}
			}

			TextRenderer.DrawText(e.Graphics, adjustedText, base.Font, ClientRectangle.Location, ForeColor, BackColor);
			
			//draw highlighted text
			if (SelectedText != string.Empty)
			{
				var startSize = TextRenderer.MeasureText(adjustedText.AsSpan()[..(SelectionStart - 1)], base.Font, ClientRectangle.Size);
				startSize.Height -= base.Font.Height;//go up one row
				TextRenderer.DrawText(e.Graphics, SelectedText, base.Font, (Point)startSize, SystemColors.HighlightText, SystemColors.Highlight);
				Debug.WriteLine("painted the selected text");
			}
			//if we have a WM_PAINT and should display a shighlight somewhere
			if (ShowHighlight && HighlightEnd > HighlightStart && HighlightStart < Text.Length && HighlightEnd < Text.Length)
			{
				//overlay the other text highlight
				//calculate text positions
				SizeF startSize = new();
				if (HighlightStart > 0)
				{
					startSize = e.Graphics.MeasureString(adjustedText[..(HighlightStart - 1)], base.Font);
					startSize.Height -= base.Font.Height + 1;//go up one row
				}//todo fix or better idea

				TextRenderer.DrawText(e.Graphics, adjustedText.AsSpan()[HighlightStart..HighlightEnd], base.Font, new Point((int)startSize.Width, (int)startSize.Height), Utils.darkText, Utils.highlight);
				Debug.WriteLine("painted the yellow highlight text");
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg != 15)
			{
				base.WndProc(ref m);
				return;
			}
			//we have a paint message, send to own handler. only if we have a gdi handle
			if (Handle != 0) OnPaint(new PaintEventArgs(Graphics.FromHwnd(m.HWnd), ClientRectangle));
		}
	}
}
