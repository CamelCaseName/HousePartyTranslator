using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("Windows")]
    internal sealed class SearchTextBox : WinTextBox
    {
        internal SearchTextBox()
        {
            PlaceholderColor = SystemColors.GrayText;
            KeyDown += (s, e) => InvalidateCounter();
        }

        public int TotalSearchResults
        {
            get
            {
                return _totalSearchResults;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(TotalSearchResults),
                        $"{nameof(TotalSearchResults)} must not be negative");
                }
                else
                {
                    _totalSearchResults = value;
                    _counter = string.Concat(_currentSearchResult.ToString(), "/", _totalSearchResults.ToString());
                }
            }
        }
        private int _totalSearchResults = 0;

        public int CurrentSearchResult
        {
            get
            {
                return _currentSearchResult;
            }

            set
            {
                if (value >= 0 && value <= _totalSearchResults)
                {
                    _currentSearchResult = value;
                    _counter = string.Concat(" ", _currentSearchResult.ToString(), "/", _totalSearchResults.ToString());
                    InvalidateCounter();
                }
            }
        }
        private int _currentSearchResult = 0;

        private string _counter = "0/0";

        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                InvalidateCounter();
            }
        }

        private void InvalidateCounter()
        {
            MeasureCounterText(out Size size, out Point pos);
            Invalidate(new Rectangle(pos, size));
        }

        private void MeasureCounterText(out Size size, out Point pos)
        {
            size = TextRenderer.MeasureText(_counter, Font);
            pos = new Point(
                            ClientRectangle.Right - size.Width,
                            (ClientRectangle.Height - size.Height) / 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            OnPaintOffset(e, new Point(2, 2));

            if (_totalSearchResults > 0)
            {
                MeasureCounterText(out _, out Point pos);
                TextRenderer.DrawText(e.Graphics, _counter, Font, pos, PlaceholderColor, BackColor);
            }
        }
    }
}
