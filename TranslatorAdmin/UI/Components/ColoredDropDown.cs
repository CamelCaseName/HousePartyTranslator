using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Helpers;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    internal sealed class ColoredDropDown : ComboBox
    {
        private int[] coloredIndices;
        public Color SpecialIndexBackColor
        {
            get
            {
                return _specialIndexBackColor;
            }
            set
            {
                _specialIndexBackColor = value;
                SpecialBackgroundBrush.Color = _specialIndexBackColor;
            }
        }
        private Color _specialIndexBackColor = Color.MediumSeaGreen;
        private readonly Pen BorderPen = new(Utils.foreground);
        private readonly Pen BlackPen = new(Utils.darkText);
        private readonly SolidBrush BlackBrush = new(Utils.darkText);
        private readonly SolidBrush BackgroundBrush = new(Utils.menu);
        private readonly SolidBrush Borderbrush = new(Utils.foreground);
        private readonly SolidBrush SpecialBackgroundBrush = new(Color.MediumSeaGreen);
        private readonly SolidBrush HighlightBrush = new(Utils.menuHighlight);

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
            int[] newArray = new int[coloredIndices.Length + 1];
            coloredIndices.CopyTo(newArray, 0);
            newArray[^1] = index;
            coloredIndices = newArray;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e is null) return;
            base.OnDrawItem(e);

            //true when we hover over the item
            if (e.State.HasFlag(DrawItemState.Focus))
            {
                e.Graphics.FillRectangle(HighlightBrush, e.Bounds);
            }
            else
            {
                if (coloredIndices.Contains(e.Index))
                {
                    e.Graphics.FillRectangle(SpecialBackgroundBrush, e.Bounds);
                }
                else
                {
                    //overdraw native drawing as its wrong lol
                    e.Graphics.FillRectangle(BackgroundBrush, e.Bounds);
                }
            }
            if (e.Index > -1)
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, new Rectangle(e.Bounds.X + 1, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), Utils.darkText, TextFormatFlags.Left);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (e is null) return;
            base.OnPaint(e);
            int h = Height;
            int w = Width;
            e.Graphics.DrawLine(BlackPen, new Point(0, h - 1), new Point(w, h - 1));
            e.Graphics.DrawRectangle(BorderPen, new Rectangle(1, 1, w - 3, h - 4));
            e.Graphics.FillRectangle(BackgroundBrush, new Rectangle(2, 2, w - 5, h - 6));
            e.Graphics.FillRectangle(Borderbrush, new Rectangle(w - 18, 2, 16, h - 5));
            e.Graphics.FillPolygon(BlackBrush, new Point[3] { new Point(w - 12, h - 14), new Point(w - 7, h - 14), new Point(w - 10, h - 11) });
            TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(1, 4, w, h - 4), ForeColor, TextFormatFlags.Left);
        }
    }
}
