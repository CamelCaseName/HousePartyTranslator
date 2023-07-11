using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    public sealed class ColoredToolStripDropDown : ToolStripControlHost
    {
        public ColoredToolStripDropDown() : base(new ColoredDropDown()) { }

        internal ColoredDropDown DropDown => Control as ColoredDropDown ?? new();

        public ComboBoxStyle DropDownStyle { get => DropDown.DropDownStyle; set => DropDown.DropDownStyle = value; }
        public FlatStyle FlatStyle { get => DropDown.FlatStyle; set => DropDown.FlatStyle = value; }
        public int MaxLength { get => DropDown.MaxLength; set => DropDown.MaxLength = value; }
        public ComboBox.ObjectCollection Items { get => DropDown.Items; }
        public object SelectedItem { get => DropDown.SelectedItem; set => DropDown.SelectedItem = value; }
        public event EventHandler SelectedIndexChanged { add => DropDown.SelectedIndexChanged += value; remove => DropDown.SelectedIndexChanged -= value; }
    }
}
