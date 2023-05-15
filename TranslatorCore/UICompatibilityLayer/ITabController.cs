using System.Collections.Generic;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ITabController<TLineItem, TTab>
        where TLineItem : class, ILineItem, new()
        where TTab : class, ITab<TLineItem>, new()
    {
        int SelectedIndex { get; set; }
        TTab SelectedTab { get; set; }
        int TabCount { get; }
        List<TTab> TabPages { get; }
        void AddTab(TTab tab);
        bool CloseTab(TTab tab);
    }
}