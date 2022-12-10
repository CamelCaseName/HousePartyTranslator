using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinTabController : TabControl, ITabController<WinLineItem>
    {
        public new int SelectedIndex { get => base.SelectedIndex; set => base.SelectedIndex = value; }
        public new ITab<WinLineItem> SelectedTab { get => (ITab<WinLineItem>)base.SelectedTab; set => base.SelectedTab = (TabPage)value; }

        public new int TabCount => TabPages.Count;

        public new List<ITab<WinLineItem>> TabPages => base.TabPages.ToTabList();

        public bool CloseTab(ITab<WinLineItem> tab) => throw new NotImplementedException();
    }
}
