using System;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.ComponentModel;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    public sealed class ColoredToolStripDropDown : ToolStripControlHost
    {
        public ColoredToolStripDropDown() : base(new ColoredDropDown()) { }

        public ColoredDropDown DropDown => Control as ColoredDropDown ?? new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboBoxStyle DropDownStyle { get => DropDown.DropDownStyle; set => DropDown.DropDownStyle = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FlatStyle FlatStyle { get => DropDown.FlatStyle; set => DropDown.FlatStyle = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxLength { get => DropDown.MaxLength; set => DropDown.MaxLength = value; }
        public ComboBox.ObjectCollection Items { get => DropDown.Items; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem { get => DropDown.SelectedItem; set => DropDown.SelectedItem = value; }
        public event EventHandler SelectedIndexChanged { add => DropDown.SelectedIndexChanged += value; remove => DropDown.SelectedIndexChanged -= value; }
    }
}
