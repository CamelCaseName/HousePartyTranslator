using System;
using System.Windows.Forms;
using Translator.Core.UICompatibilityLayer;
using System.ComponentModel;

namespace Translator.Desktop.UI.Components
{
    public class WinMenuSeperator : ToolStripSeparator, IMenuItem
    {
        public new event EventHandler Click { add => base.Click += value; remove => base.Click -= value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text { get => base.Text; set => base.Text = value; }
    }
}
