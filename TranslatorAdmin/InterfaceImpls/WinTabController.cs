using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Desktop.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    public class WinTabController : TabControl, ITabController
    {
        public new int SelectedIndex { get => base.SelectedIndex; set => base.SelectedIndex = value; }
        public new ITab SelectedTab { get => (ITab)base.SelectedTab; set => base.SelectedTab = (TabPage)value; }

        public new int TabCount => TabPages.Count;

        public new List<ITab> TabPages => base.TabPages.ToTabList();

        public void AddTab(ITab tab) => base.TabPages.Add((TabPage)tab);
        public bool CloseTab(ITab tab)
        {
            base.TabPages.Remove((TabPage)tab);
            return true;
        }
    }
}
