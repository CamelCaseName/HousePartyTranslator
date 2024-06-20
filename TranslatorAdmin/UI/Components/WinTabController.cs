
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.Foundation;
using Translator.Desktop.Helpers;

namespace Translator.Desktop.UI.Components
{

    [SupportedOSPlatform("Windows")]
    public class WinTabController : WinTabController<WinTab>, ITabController
    {
        ITab ITabController<ITab>.SelectedTab { get => SelectedTab; set => SelectedTab = (WinTab)value; }

        List<ITab> ITabController<ITab>.TabPages => TabPages.Cast<ITab>().ToList();

        void ITabController<ITab>.AddTab(ITab tab) => AddTab((WinTab)tab);
        bool ITabController<ITab>.CloseTab(ITab tab) => CloseTab((WinTab)tab);
    }


    [SupportedOSPlatform("Windows")]
    public class WinTabController<X> : TabControl where X : TabPage
    {
        private readonly SolidBrush background = new(Utils.menu);
        private readonly SolidBrush greyedBackground = new(Utils.background);
        private readonly SolidBrush blackBackground = new(Utils.darkText);

        public WinTabController() : base()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += DrawTabTitleCards;
            Padding = Point.Empty;
            Margin = System.Windows.Forms.Padding.Empty;
        }

        private void DrawTabTitleCards(object? sender, DrawItemEventArgs e)
        {
            if (sender is null)
                return;
            if (e.Index < 0)
                return;

            Font font = TabPages[e.Index].Text.Contains('*') ? new Font(Font, FontStyle.Bold) : new Font(Font, FontStyle.Regular);

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
        public new X SelectedTab { get => (X)base.SelectedTab; set => base.SelectedTab = value; }

        public new int TabCount => TabPages.Count;

        public new List<X> TabPages => base.TabPages.ToTabList<X>();

        public void AddTab(X tab)
        {
            base.TabPages.Add(tab);
        }

        public bool CloseTab(X tab)
        {
            base.TabPages.Remove(tab);
            return true;
        }


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Winutils.WM_PAINT)
            {
                var graphics = Graphics.FromHwnd(m.HWnd);
                var rect = new RECT(ClientRectangle);

                graphics.Clear(Utils.darkText);

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
