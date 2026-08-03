using System.Runtime.Versioning;
using System.ComponentModel;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("Windows")]
    public sealed class SearchToolStripTextBox : ToolStripControlHost
    {
        public SearchToolStripTextBox() : base(new SearchTextBox()) { }

        public SearchTextBox TextBox => Control as SearchTextBox ?? new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text { get => TextBox.Text; set => TextBox.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TotalSearchResults { get => TextBox.TotalSearchResults; set => TextBox.TotalSearchResults = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentSearchResult { get => TextBox.CurrentSearchResult; set => TextBox.CurrentSearchResult = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart { get => TextBox.SelectionStart; set => TextBox.SelectionStart = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionEnd { get => TextBox.SelectionEnd; set => TextBox.SelectionEnd = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength { get => TextBox.SelectionLength; set => TextBox.SelectionLength = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BorderStyle BorderStyle { get => TextBox.BorderStyle; set => TextBox.BorderStyle = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor { get => TextBox.BackColor; set => TextBox.BackColor = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color ForeColor { get => TextBox.ForeColor; set => TextBox.ForeColor = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Font Font { get => TextBox.Font; set => TextBox.Font = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size { get => TextBox.Size; set => TextBox.Size = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size MinimumSize { get => TextBox.MinimumSize; set => TextBox.MinimumSize = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size MaximumSize { get => TextBox.MaximumSize; set => TextBox.MaximumSize = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Margin { get => TextBox.Margin; set => TextBox.Margin = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PlaceHoldeText { get => TextBox.PlaceholderText; set => TextBox.PlaceholderText = value; }
    }
}
