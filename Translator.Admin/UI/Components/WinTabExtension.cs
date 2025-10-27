using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using static System.Windows.Forms.TabControl;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    internal static class WinTabExtension
    {
        public static TabPageCollection ToTabPageCollection(this List<WinTab> collection, TabControl owner)
        {
            var items = new TabPageCollection(owner);
            for (int i = 0; i < collection.Count; i++)
            {
                items.Add(collection[i]);
            }
            return items;
        }
    }
}
