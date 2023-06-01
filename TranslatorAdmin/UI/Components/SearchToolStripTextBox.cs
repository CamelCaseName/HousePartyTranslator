using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("Windows")]
    internal sealed class SearchToolStripTextBox : ToolStripControlHost
    {
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
                if (value > 0 && value < _totalSearchResults)
                {
                    _currentSearchResult = value;
                    _counter = string.Concat(_currentSearchResult.ToString(), "/", _totalSearchResults.ToString());
                }
            }
        }
        private int _currentSearchResult = 0;

        public string Counter => _counter;
        private string _counter = "0/0";

        public SearchToolStripTextBox() : base(new WinTextBox()) {}

        public WinTextBox TextBox => Control as WinTextBox ?? new();

        public new string Text { get => TextBox.Text; set => TextBox.Text = value; }
        public int SelectionStart { get => TextBox.SelectionStart; set => TextBox.SelectionStart = value; }
        public int SelectionEnd { get => TextBox.SelectionEnd; set => TextBox.SelectionEnd = value; }
        public int SelectionLength { get => TextBox.SelectionLength; set => TextBox.SelectionLength = value; }
        public BorderStyle BorderStyle { get => TextBox.BorderStyle; set => TextBox.BorderStyle = value; }
        public new Color BackColor { get => TextBox.BackColor; set => TextBox.BackColor = value; }
        public new Color ForeColor { get => TextBox.ForeColor; set => TextBox.ForeColor = value; }
        public new Font Font { get => TextBox.Font; set => TextBox.Font = value; }
        public new Size Size { get => TextBox.Size; set => TextBox.Size = value; }
        public Size MinimumSize { get => TextBox.MinimumSize; set => TextBox.MinimumSize = value; }
        public Size MaximumSize { get => TextBox.MaximumSize; set => TextBox.MaximumSize = value; }
        public new Padding Margin { get => TextBox.Margin; set => TextBox.Margin = value; }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_totalSearchResults > 0)
            {
                var size = TextRenderer.MeasureText(_counter, Font);
                var pos = new Point(
                    ContentRectangle.Right - size.Width + Bounds.Left,
                    ContentRectangle.Top - ((ContentRectangle.Height - size.Height) / 2) + Bounds.Top);
                TextRenderer.DrawText(e.Graphics, _counter, Font, pos, ForeColor, BackColor);
            }
        }
    }
}
