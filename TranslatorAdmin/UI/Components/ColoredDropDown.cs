using System.Drawing;
using System.Windows.Forms;
using Translator.Core.Helpers;

namespace Translator.Desktop.UI.Components
{
    internal class ColoredDropDown : ComboBox
    {
        private int[] coloredIndices;
        public Color SpecialIndexBackColor { get; set; } = Color.MediumSeaGreen;

        public ColoredDropDown(int[] coloredIndices)
        {
            this.coloredIndices = coloredIndices;
        }

        public ColoredDropDown(int length) : this(new int[length]) { }

        public ColoredDropDown() : this(System.Array.Empty<int>()) { }

        public void SetColoredIndices(int[] indices)
        {
            coloredIndices = indices;
        }

        public void AddIndex(int index)
        {
            var newArray = new int[coloredIndices.Length + 1];
            coloredIndices.CopyTo(newArray, 0);
            newArray[^1] = index;
            coloredIndices = newArray;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e == null) return;
            var color = e.BackColor;

            if (coloredIndices.Contains(e.Index))
                color = SpecialIndexBackColor;

            var e2 = new DrawItemEventArgs(
                e.Graphics,
                e.Font,
                e.Bounds,
                e.Index,
                e.State,
                e.ForeColor,
                color);
            base.OnDrawItem(e2);
        }
    }
}
