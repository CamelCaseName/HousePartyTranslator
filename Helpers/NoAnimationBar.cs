using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.Helpers
{
    public class NoAnimationBar : ProgressBar
    {
        public NoAnimationBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ForeColor), new Rectangle(0, 0, (int)(Width * (float)Value / (float)Maximum), Height));
        }
    }
}
