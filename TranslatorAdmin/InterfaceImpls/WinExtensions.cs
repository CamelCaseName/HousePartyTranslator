using System.Runtime.Versioning;
using Translator.Core.Helpers;
using Translator.Explorer;
using Translator.UICompatibilityLayer;
using static System.Windows.Forms.TabControl;

namespace TranslatorApp.InterfaceImpls
{
	[SupportedOSPlatform("Windows")]
	internal static class WinExtensions
	{
		internal static PopupResult ToPopupResult(this DialogResult result)
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

		internal static DialogResult ToDialogResult(this PopupResult result)
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

		/// <summary>
		/// Gets the category of a node from a node type
		/// </summary>
		/// <param name="type">the type to get the stringcategory form</param>
		/// <returns>the corresponding stringcategory</returns>
		internal static StringCategory CategoryFromNode(this NodeType type)
		{
			return type switch
			{
				NodeType.Null => StringCategory.Neither,
				NodeType.Item => StringCategory.ItemName,
				NodeType.ItemGroup => StringCategory.ItemGroupAction,
				NodeType.Action => StringCategory.ItemAction,
				NodeType.Event => StringCategory.Event,
				NodeType.Criterion => StringCategory.Neither,
				NodeType.Response => StringCategory.Response,
				NodeType.Dialogue => StringCategory.Dialogue,
				NodeType.Quest => StringCategory.Quest,
				NodeType.Achievement => StringCategory.Achievement,
				NodeType.Reaction => StringCategory.Response,
				NodeType.BGC => StringCategory.BGC,
				_ => StringCategory.Neither,
			};
		}

		internal static MenuItems ToMenuItems(this ToolStripItemCollection collection)
		{
			var items = new MenuItems(collection.Count);
			for (int i = 0; i < collection.Count; i++)
			{
				items.Add((IMenuItem)collection[i]);
			}
			return items;
		}

		internal static ToolStripItemCollection ToToolStripItemCollection(this MenuItems collection, ToolStrip owner)
		{
			var items = new ToolStripItem[collection.Count];
			for (int i = 0; i < collection.Count; i++)
			{
				items[i] = (ToolStripItem)collection[i];
			}
			return new ToolStripItemCollection(owner, items);
		}

		internal static List<WinTab> ToTabList(this TabPageCollection collection)
		{
			var items = new List<WinTab>(collection.Count);
			for (int i = 0; i < collection.Count; i++)
			{
				items.Add((WinTab)collection[i]);
			}
			return items;
		}

		internal static TabPageCollection ToTabPageCollection(this List<WinTab> collection, TabControl owner)
		{
			var items = new TabPageCollection(owner);
			for (int i = 0; i < collection.Count; i++)
			{
				items.Add(collection[i]);
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
