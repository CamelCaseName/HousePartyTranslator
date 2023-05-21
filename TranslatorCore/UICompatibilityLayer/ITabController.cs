using System.Collections.Generic;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ITabController
    {
        int SelectedIndex { get; set; }
        ITab SelectedTab { get; set; }
        int TabCount { get; }
        List<ITab> TabPages { get; }
        void AddTab(ITab tab);
        bool CloseTab(ITab tab);
    }
}