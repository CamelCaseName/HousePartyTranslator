using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Helpers;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    internal sealed class ColoredDropDown : ComboBox
    {
        private int[] coloredIndices;
        public Color SpecialIndexBackColor { get; set; } = Color.MediumSeaGreen;
        private Pen BorderPen = new(Utils.foreground);
        private Pen BlackPen = new(Utils.darkText);
        private SolidBrush BlackBrush = new(Utils.darkText);
        private SolidBrush BackgroundBrush = new(Utils.menu);
        private SolidBrush Borderbrush = new(Utils.foreground);
        private SolidBrush SpecialBackgroundBrush = new(Color.MediumSeaGreen);

        public ColoredDropDown(int[] coloredIndices)
        {
            this.coloredIndices = coloredIndices;
            SetStyle(ControlStyles.UserPaint, true);
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        public ColoredDropDown(int length) : this(new int[length]) { }

        public ColoredDropDown() : this(Array.Empty<int>()) { }

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
            base.OnDrawItem(e);

            if (coloredIndices.Contains(e.Index))
            {
                e.Graphics.FillRectangle(SpecialBackgroundBrush, e.Bounds);
            }
            TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, new Rectangle(e.Bounds.X + 1, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), Utils.darkText, TextFormatFlags.Left);
            //todo add blue hover effect back in
            //todo remove font changing on hover
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (e == null) return;
            base.OnPaint(e);
            e.Graphics.DrawLine(BlackPen, new Point(0, Height - 1), new Point(Width, Height - 1));
            e.Graphics.DrawRectangle(BorderPen, new Rectangle(1, 1, Width - 3, Height - 4));
            e.Graphics.FillRectangle(BackgroundBrush, new Rectangle(2,  2, Width - 5, Height - 6));
            e.Graphics.FillRectangle(Borderbrush, new Rectangle(Width - 18,  2, 16, Height - 5));
            e.Graphics.FillPolygon(BlackBrush, new Point[3] { new Point(Width - 12, Height - 14), new Point(Width - 7, Height - 14), new Point(Width - 10, Height - 11) });
            TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(1,  4, Width, Height - 4), ForeColor, TextFormatFlags.Left);
        }
    }
}
