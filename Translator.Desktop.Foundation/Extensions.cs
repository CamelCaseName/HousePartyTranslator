using Translator.Core.Data;
using static System.Windows.Forms.TabControl;
using Translator.Core.UICompatibilityLayer;
using System.Runtime;

namespace Translator.Desktop.Foundation
{
    public static class Extensions
    {
        public static PopupResult ToPopupResult(this DialogResult result)
        {
            return result switch
            {
                DialogResult.None => PopupResult.NONE,
                DialogResult.OK => PopupResult.OK,
                DialogResult.Cancel => PopupResult.CANCEL,
                DialogResult.Abort => PopupResult.ABORT,
                DialogResult.Retry => PopupResult.NONE,
                DialogResult.Ignore => PopupResult.IGNORE,
                DialogResult.Yes => PopupResult.YES,
                DialogResult.No => PopupResult.NO,
                DialogResult.TryAgain => PopupResult.NONE,
                DialogResult.Continue => PopupResult.CONTINUE,
                _ => PopupResult.NONE,
            };
        }

        public static DialogResult ToDialogResult(this PopupResult result)
        {
            return result switch
            {
                PopupResult.NONE => DialogResult.None,
                PopupResult.OK => DialogResult.OK,
                PopupResult.YES => DialogResult.Yes,
                PopupResult.NO => DialogResult.No,
                PopupResult.CANCEL => DialogResult.Cancel,
                PopupResult.ABORT => DialogResult.Abort,
                PopupResult.CONTINUE => DialogResult.Continue,
                PopupResult.IGNORE => DialogResult.Ignore,
                _ => DialogResult.None,
            };
        }
        public static MenuItems ToMenuItems(this ToolStripItemCollection collection)
        {
            var items = new MenuItems(collection.Count);
            for (int i = 0; i < collection.Count; i++)
            {
                items.Add((IMenuItem)collection[i]);
            }
            return items;
        }

        public static ToolStripItemCollection ToToolStripItemCollection(this MenuItems collection, ToolStrip owner)
        {
            var items = new ToolStripItem[collection.Count];
            for (int i = 0; i < collection.Count; i++)
            {
                items[i] = (ToolStripItem)collection[i];
            }
            return new ToolStripItemCollection(owner, items);
        }

        public static List<X> ToTabList<X>(this TabPageCollection collection) where X : TabPage
        {
            var items = new List<X>(collection.Count);
            for (int i = 0; i < collection.Count; i++)
            {
                items.Add((X)collection[i]);
            }
            return items;
        }

        public static List<ITab> ToTabList(this TabPageCollection collection)
        {
            var items = new List<ITab>(collection.Count);
            for (int i = 0; i < collection.Count; i++)
            {
                items.Add((ITab)collection[i]);
            }
            return items;
        }

        public static void AddRangeFix(this ToolStripItemCollection collection, ToolStripItemCollection toolStripItems)
        {
            for (int i = 0; i < toolStripItems.Count; i++)
            {
                _ = collection.Add(toolStripItems[i]);
                collection[^1].Owner = collection[0].Owner;
            }
        }

        public static void AddRangeFix(this ToolStripItemCollection collection, ToolStripItem[] toolStripItems)
        {
            for (int i = 0; i < toolStripItems.Length; i++)
            {
                _ = collection.Add(toolStripItems[i]);
                collection[^1].Owner = collection[0].Owner;
            }
        }
    }
}
