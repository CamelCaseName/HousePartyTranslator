using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.Helpers;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    public class WinTextBox : TextBox, ITextBox
    {
        private bool customDrawNeeded = false;
        private bool showHighlight = false;

        public WinTextBox() : base()
        {
            MouseDown += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            MouseUp += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            MouseDoubleClick += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            GotFocus += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            Click += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            Resize += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            LostFocus += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            MouseCaptureChanged += (sender, e) => { customDrawNeeded = true; Invalidate(); };
            MouseMove += (sender, e) => _ = e.Button == MouseButtons.Left ? customDrawNeeded = true : customDrawNeeded = false;
        }

        public int SelectionEnd
        {
            get => base.SelectionStart + base.SelectionLength;
            set { if (SelectionStart <= SelectionEnd) base.SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
        }

        public new int SelectionStart { get => base.SelectionStart; set => base.SelectionStart = value; }

        public int HighlightStart { get; set; }

        public int HighlightEnd { get; set; }

        public bool ShowHighlight
        {
            get => showHighlight;
            set { Invalidate(); showHighlight = value; customDrawNeeded = true; }
        }

        public new string Text
        {
            get => base.Text;
            set { Invalidate(); customDrawNeeded = true; base.Text = value; }
        }

        public Color PlaceholderColor { get; set; } = SystemColors.GrayText;

        public new void Focus() => base.Focus();

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if ((m.Msg == Winutils.WM_PAINT || (m.Msg == Winutils.WM_MOUSEMOVE && customDrawNeeded)) && IsHandleCreated)
                //we have a paint message, send to own handler. only if we have a gdi handle
                OnPaint(new PaintEventArgs(Graphics.FromHwnd(m.HWnd), ClientRectangle));
        }

        public void OnPaintOffset(PaintEventArgs e, Point offset)
        {
            //let the base handle backgorund and border drawing and so on
            base.OnPaint(e);
            //if the control isnt focused and we can draw a placeholdertext, do it
            if (ShouldDrawPlaceholder())
            {
                DrawPlaceholderText(e.Graphics, offset);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //let the base handle backgorund and border drawing and so on
            base.OnPaint(e);
            //if the control isnt focused and we can draw a placeholdertext, do it
            if (ShouldDrawPlaceholder())
            {
                DrawPlaceholderText(e.Graphics);
            }
            //if we have a WM_PAINT and should display a shighlight somewhere
            if (ShouldDrawHighlight())
            {
                DrawHighlightedText(e.Graphics);
            }
        }

        private bool ShouldDrawPlaceholder()
        {
            return
            !string.IsNullOrEmpty(PlaceholderText)
            && !Focused
            && TextLength == 0;
        }

        private bool ShouldDrawHighlight()
        {
            return
            customDrawNeeded
            && ShowHighlight
            && HighlightEnd > HighlightStart
            && HighlightStart < Text.Length
            && HighlightEnd <= Text.Length;
        }

        private void DrawPlaceholderText(Graphics g) => DrawPlaceholderText(g, Point.Empty);

        private void DrawHighlightedText(Graphics g)
        {
            //overlay the other text highlight
            //get text positions
            int currentHighlightLength = 0, newStartPos = HighlightStart;
            while (newStartPos + currentHighlightLength < HighlightEnd)
            {
                //find length of search term in the current line
                currentHighlightLength = Text.AsSpan()[newStartPos..HighlightEnd].IndexOfAny(" ,.-".AsSpan());
                if (currentHighlightLength < 0)
                    currentHighlightLength = HighlightEnd - newStartPos;
                else
                    ++currentHighlightLength;//so that we also contain the other character

                Point highlightLocation = GetPositionFromCharIndex(newStartPos);
                //adjust offset
                highlightLocation.X -= 2;

                //render highlight in the current line
                AdjustTextRegion(out TextFormatFlags flags, ref highlightLocation);
                TextRenderer.DrawText(g, Text.AsSpan()[newStartPos..(newStartPos + currentHighlightLength)], base.Font, highlightLocation, Utils.darkText, Utils.highlight, flags);
                //move over
                newStartPos += currentHighlightLength;
                currentHighlightLength = 0;
            }
            customDrawNeeded = false;
        }

        private void DrawPlaceholderText(Graphics g, Point offset)
        {
            AdjustTextRegion(out TextFormatFlags flags, ref offset);

            TextRenderer.DrawText(g, PlaceholderText, Font, offset, PlaceholderColor, BackColor, flags);
        }

        private void AdjustTextRegion(out TextFormatFlags flags, ref Point point)
        {
            flags = TextFormatFlags.NoPadding | TextFormatFlags.Top |
                                                TextFormatFlags.EndEllipsis;
            if (point.IsEmpty) point = ClientRectangle.Location;
            if (RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
                switch (TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags |= TextFormatFlags.HorizontalCenter;
                        point.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags |= TextFormatFlags.Right;
                        point.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags |= TextFormatFlags.Left;
                        point.Offset(0, 1);
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
                        point.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags |= TextFormatFlags.Left;
                        point.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags |= TextFormatFlags.Right;
                        point.Offset(0, 1);
                        break;
                }
            }
        }
    }
}
