using System.Runtime.Versioning;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Silk.NET.OpenCL;

namespace Translator.InterfaceImpls
{
    [SupportedOSPlatform("windows")]
    public class WinTextBox : TextBox, ITextBox
    {
        private const int WM_PAINT = 15;
        private const int WM_MOUSEMOVE = 512;
        private bool customDrawNeeded = false;
        private bool showHighlight = false;

        public WinTextBox() : base()
        {
            MouseDown += (object? sender, MouseEventArgs e) => { customDrawNeeded = true; Invalidate(); };
            MouseUp += (object? sender, MouseEventArgs e) => { customDrawNeeded = true; Invalidate(); };
            MouseDoubleClick += (object? sender, MouseEventArgs e) => { customDrawNeeded = true; Invalidate(); };
            GotFocus += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
            Click += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
            Resize += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
            LostFocus += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
            MouseCaptureChanged += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
            MouseMove += (object? sender, MouseEventArgs e) => _ = e.Button == MouseButtons.Left ? customDrawNeeded = true : customDrawNeeded = false;
        }

        public int SelectionEnd
        {
            get => base.SelectionStart + base.SelectionLength;
            set { if (SelectionStart <= SelectionEnd) base.SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
        }

        public new int SelectionStart { get => base.SelectionStart; set => base.SelectionStart = value; }

        public int HighlightStart { get; set; }

        public int HighlightEnd { get; set; }

        public bool ShowHighlight { get => showHighlight; set { Invalidate(); showHighlight = value; customDrawNeeded = true; } }

        public new string Text { get => base.Text; set { Invalidate(); customDrawNeeded = true; base.Text = value; } }

        public new void Focus() => base.Focus();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!string.IsNullOrEmpty(PlaceholderText) && !Focused && TextLength == 0)
            {
                DrawPlaceholderText(e.Graphics);
            }
            //if we have a WM_PAINT and should display a shighlight somewhere
            else if (customDrawNeeded && ShowHighlight && HighlightEnd > HighlightStart && HighlightStart < Text.Length && HighlightEnd <= Text.Length)
            {
                DrawHighlightedText(e.Graphics);
            }
        }

        private void DrawHighlightedText(Graphics g)
        {
            //overlay the other text highlight
            //get text positions
            int currentHighlightLength = 0, newStartPos = HighlightStart;
            while (newStartPos + currentHighlightLength < HighlightEnd)
            {
                currentHighlightLength = Text.AsSpan()[newStartPos..HighlightEnd].IndexOfAny(" ,.-".AsSpan());
                if (currentHighlightLength < 0)
                    currentHighlightLength = HighlightEnd - newStartPos;
                else
                    ++currentHighlightLength;//so that we also contain the other character

                Point highlightLocation = GetPositionFromCharIndex(newStartPos);
                highlightLocation.X -= 2;
                TextRenderer.DrawText(g, Text.AsSpan()[newStartPos..(newStartPos + currentHighlightLength)], base.Font, highlightLocation, Utils.darkText, Utils.highlight);
                //move over
                newStartPos += currentHighlightLength;
                currentHighlightLength = 0;
            }
            customDrawNeeded = false;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if ((m.Msg == WM_PAINT || m.Msg == WM_MOUSEMOVE) && IsHandleCreated)
                //we have a paint message, send to own handler. only if we have a gdi handle
                OnPaint(new PaintEventArgs(Graphics.FromHwnd(m.HWnd), ClientRectangle));

        }

        private void DrawPlaceholderText(Graphics g)
        {
            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top |
                                    TextFormatFlags.EndEllipsis;
            Rectangle rectangle = ClientRectangle;

            if (RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
                switch (TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags |= TextFormatFlags.HorizontalCenter;
                        rectangle.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags |= TextFormatFlags.Right;
                        rectangle.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags |= TextFormatFlags.Left;
                        rectangle.Offset(0, 1);
                        break;
                }
            }
            else
            {
                flags &= ~TextFormatFlags.RightToLeft;
                switch (TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags |= TextFormatFlags.HorizontalCenter;
                        rectangle.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags |= TextFormatFlags.Left;
                        rectangle.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags |= TextFormatFlags.Right;
                        rectangle.Offset(0, 1);
                        break;
                }
            }

            TextRenderer.DrawText(g, PlaceholderText, Font, rectangle, Utils.darkText, Utils.background, flags);
        }
    }
}
