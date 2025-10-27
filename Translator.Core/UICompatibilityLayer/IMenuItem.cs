using System;

namespace Translator.Core.UICompatibilityLayer
{
    public interface IMenuItem
    {
        public event EventHandler Click { add { } remove { } }
        public string Text { get; set; }
    }
}