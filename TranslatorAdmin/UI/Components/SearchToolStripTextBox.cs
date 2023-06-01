using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("Windows")]
    internal sealed class SearchToolStripTextBox : ToolStripControlHost
    {
        public SearchToolStripTextBox() : base(new SearchTextBox()) { }

        public SearchTextBox TextBox => Control as SearchTextBox ?? new();

        public new string Text { get => TextBox.Text; set => TextBox.Text = value; }
        public int TotalSearchResults { get => TextBox.TotalSearchResults; set => TextBox.TotalSearchResults = value; }
        public int CurrentSearchResult { get => TextBox.CurrentSearchResult; set => TextBox.CurrentSearchResult = value; }
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
        public string PlaceHoldeText { get => TextBox.PlaceholderText; set => TextBox.PlaceholderText = value; }
    }
}
