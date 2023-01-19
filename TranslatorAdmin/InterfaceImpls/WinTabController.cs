using System.Runtime.Versioning;
using Translator.UICompatibilityLayer;

namespace TranslatorApp.InterfaceImpls
{
	[SupportedOSPlatform("Windows")]
	public class WinTabController : TabControl, ITabController<WinLineItem, WinTab>
	{
		public new int SelectedIndex { get => base.SelectedIndex; set => base.SelectedIndex = value; }
		public new WinTab SelectedTab { get => (WinTab)base.SelectedTab; set => base.SelectedTab = value; }

		public new int TabCount => TabPages.Count;

		public new List<WinTab> TabPages => base.TabPages.ToTabList();

		public void AddTab(WinTab tab) => base.TabPages.Add(tab);
		public bool CloseTab(WinTab tab)
		{
			base.TabPages.Remove(tab);
			return true;
		}
	}
}
