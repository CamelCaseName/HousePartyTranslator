using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.TabControl;
using System.Windows.Forms;

namespace Translator.Desktop.UI.Components
{
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
