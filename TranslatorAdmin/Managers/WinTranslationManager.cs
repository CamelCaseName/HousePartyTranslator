using Translator.Core.Helpers;
using Translator.Explorer;
using Translator.Explorer.JSON;
using Translator.Helpers;
using Translator.InterfaceImpls;
using TabManager = Translator.Core.TabManager<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;
using TranslationManager = Translator.Core.TranslationManager<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;

namespace Translator.Managers
{
	//also contains some extensions to ease programming
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	public static class WinTranslationManager
	{
		public static bool UpdateStoryExplorerSelection { get; internal set; } = true;

		public static WinUIHandler? UI { get; internal set; }

		public static DiscordPresenceManager? DiscordPresence { get; internal set; }

		internal static bool CreateTemplateFromStory(string story, string filename, string path, out FileData data)
		{
			if (UI == null)
			{
				data = new();
				return false;
			}

			UI.SignalUserWait();
			data = new FileData();
			var explorer = new ContextProvider(new(), story == Path.GetFileNameWithoutExtension(path), false, filename, story, path);
			NodeList nodes = explorer.GetTemplateNodes();
			if (nodes != null)
			{
				data = new FileData
				{
                    //add name as first template (its not in the file)
                    { "Name", new LineData("Name", story, filename, StringCategory.General, filename) }
				};

				//Add all new lines, but check if they are relevant
				for (int i = 0; i < nodes.Count; i++)
				{
					//filter out irrelevant nodes
					//todo add filter for text output events so it doesnt get the standard event stuff
					if (!((int.TryParse(nodes[i].Text, out _) || nodes[i].Text.Length < 2)
						&& nodes[i].Type == NodeType.Event)
						&& nodes[i].Type.CategoryFromNode() != StringCategory.Neither
						&& nodes[i].ID != ""
						&& nodes[i].ID != null)
					{
						data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].Text, true);
					}
				}

				UI.SignalUserEndWait();

				return true;
			}

			_ = Msg.ErrorOk("Something broke, please try again.");
			UI.SignalUserEndWait();
			return false;
		}

		/// <summary>
		/// Sets the node whose tree gets highlighted to the one representing the currently selected string;
		/// </summary>
		internal static void SetHighlightedNode(this TranslationManager manager)
		{
			if (TabManager.UI == null) return;
			if (manager.TranslationData.Count > 0)
			{
				int currentIndex = App.MainForm.Invoke(() => TabManager.UI.SelectedTab.SelectedLineIndex);
				string id = App.MainForm.Invoke(() => currentIndex < manager.TranslationData.Count && currentIndex >= 0 ? manager.TranslationData[manager.SelectedId].ID : "Name");
				//Highlights the node representign the selected string in the story explorer window
				if (App.MainForm?.Explorer != null && !App.MainForm.Explorer.IsDisposed)
				{
					App.MainForm.Invoke(() => App.MainForm.Explorer.Grapher.HighlightedNode = App.MainForm.Explorer?.Provider.Nodes.Find(n => n.ID == id) ?? Node.NullNode);
				}
			}
		}
	}
}
