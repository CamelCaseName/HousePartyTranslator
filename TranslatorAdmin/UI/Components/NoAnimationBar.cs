namespace Translator.Desktop.UI.Components
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

        public int SecondValue { get; set; }

        /// <summary>
        /// A override for the OnPaint method to draw the bar without an animation and a custom colour.
        /// </summary>
        /// <param name="e">The PaintEventArgs to manipulate.</param>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ForeColor), new Rectangle(0, 0, (int)(Width * (float)Value / Maximum), Height));
            //e.Graphics.FillRectangle(new SolidBrush(ForeColor), new Rectangle(0, 0, (int)(Width * (float)SecondValue / Maximum), Height / 2));
            //e.Graphics.FillRectangle(new SolidBrush(ForeColor), new Rectangle(0, Height / 2, (int)(Width * (float)Value / Maximum), Height / 2));
        }
    }
}