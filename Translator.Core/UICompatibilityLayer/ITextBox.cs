using System;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ITextBox
    {
        public event EventHandler Click { add { } remove { } }
        public event EventHandler TextChanged { add { } remove { } }
        public int SelectionEnd { get; set; }
        public int SelectionStart { get; set; }
        public int HighlightStart { get; set; }
        public int HighlightEnd { get; set; }
        public bool ShowHighlight { get; set; }
        public string Text { get; set; }
        public void Focus();
    }
}