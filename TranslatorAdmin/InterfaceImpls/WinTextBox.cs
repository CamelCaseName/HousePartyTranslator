using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinTextBox : TextBox, ITextBox
    {
        public int SelectionEnd
        {
            get => base.SelectionStart + SelectionLength;
            set { if (SelectionStart <= SelectionEnd) SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
        }
        public new int SelectionStart { get => base.SelectionStart; set => base.SelectionStart = value; }
        public new string Text { get => base.Text; set => base.Text = value; }

        public new void Focus() => base.Focus();
    }
}
