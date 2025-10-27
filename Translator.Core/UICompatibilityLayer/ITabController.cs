using System.Collections.Generic;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ITabController : ITabController<ITab> { }

    public interface ITabController<X> where X : ITab
    {
        int SelectedIndex { get; set; }
        X SelectedTab { get; set; }
        int TabCount { get; }
        List<X> TabPages { get; }
        void AddTab(X tab);
        bool CloseTab(X tab);
    }
}