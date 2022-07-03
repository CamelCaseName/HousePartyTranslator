using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.Helpers
{
    /// <summary>
    /// A extended WinForms ProgressBar which has a different colour and removes the animation.
    /// </summary>
    public class NoAnimationBar : ProgressBar
    {
        /// <summary>
        /// The constructor for said Progressbar, mimics the WinForm constructor.
        /// </summary>
        public NoAnimationBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        /// A override for the OnPaint method to draw the bar without an animation and a custom colour.
        /// </summary>
        /// <param name="e">The PaintEventArgs to manipulate.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ForeColor), new Rectangle(0, 0, (int)(Width * (float)Value / Maximum), Height));
        }
    }
}