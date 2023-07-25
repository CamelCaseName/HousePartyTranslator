using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.Helpers;
using Translator.Desktop.InterfaceImpls;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("Windows")]
    public class WinTabController : TabControl, ITabController
    {
        private readonly SolidBrush background = new(Utils.menu);
        private readonly SolidBrush greyedBackground = new(Utils.background);
        private readonly SolidBrush blackBackground = new(Utils.darkText);
        private int backgroundRedrawTabCount = 0;
        private bool redrawNeeded = false;

        public WinTabController() : base()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += DrawTabTitleCards;
            Padding = Point.Empty;
            Margin = System.Windows.Forms.Padding.Empty;
            Resize += RedrawClean;
            FontChanged += RedrawClean;
            Layout += RedrawClean;
        }

        private void RedrawClean(object? sender, System.EventArgs e)
        {
            redrawNeeded = true;
            Invalidate();
        }

        private void DrawTabTitleCards(object? sender, DrawItemEventArgs e)
        {
            if (sender == null) return;
            if (e.Index < 0) return;

            Font font;
            if (TabPages[e.Index].Text.Contains('*'))
                font = new Font(Font, FontStyle.Bold);
            else
                font = new Font(Font, FontStyle.Regular);

            //backgrounds
            if (e.State == DrawItemState.Selected)
            {
                var newBounds = new Rectangle(e.Bounds.X - 2, 0, e.Bounds.Width + 4, 24);
                e.Graphics.FillRectangle(background, newBounds);
                TextRenderer.DrawText(e.Graphics, TabPages[e.Index].Text, font, newBounds, Utils.darkText);
            }
            else
            {
                var newBounds = new Rectangle(e.Bounds.X, 4, e.Bounds.Width, 20);
                //remove larger selection if we are the last or first box
                if (e.Index == 0)
                    e.Graphics.FillRectangle(blackBackground, new Rectangle(e.Bounds.X - 2, 0, e.Bounds.Width, 24));
                else if (e.Index == TabCount - 1)
                    e.Graphics.FillRectangle(blackBackground, new Rectangle(e.Bounds.X, 0, e.Bounds.Width + 2, 24));
                else
                    e.Graphics.FillRectangle(blackBackground, new Rectangle(e.Bounds.X, 0, e.Bounds.Width, 24));

                e.Graphics.FillRectangle(greyedBackground, newBounds);
                TextRenderer.DrawText(e.Graphics, TabPages[e.Index].Text, font, newBounds, Utils.darkText);
            }
        }

        public new int SelectedIndex { get => base.SelectedIndex; set => base.SelectedIndex = value; }
        public new ITab SelectedTab { get => (ITab)base.SelectedTab; set => base.SelectedTab = (TabPage)value; }

        public new int TabCount => TabPages.Count;

        public new List<ITab> TabPages => base.TabPages.ToTabList();

        public void AddTab(ITab tab)
        {
            base.TabPages.Add((TabPage)tab);
            ((TabPage)tab).TextChanged += RedrawClean;
        }

        public bool CloseTab(ITab tab)
        {
            base.TabPages.Remove((TabPage)tab);
            return true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Winutils.WM_PAINT)
            {
                var graphics = Graphics.FromHwnd(m.HWnd);
                var rect = new RECT(ClientRectangle);
                //only redraw if we add or remove pages
                if (TabCount != backgroundRedrawTabCount || redrawNeeded)
                {
                    backgroundRedrawTabCount = TabCount;
                    graphics.Clear(Utils.darkText);
                    redrawNeeded = false;
                }
                for (int i = 0; i < TabCount; i++)
                {
                    DrawTabTitleCards(this, new DrawItemEventArgs(graphics, Font, GetTabRect(i), i, SelectedIndex == i ? DrawItemState.Selected : DrawItemState.Default));
                }
                m.Result = 0;
                Winutils.ValidateRect(m.HWnd, ref rect);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
