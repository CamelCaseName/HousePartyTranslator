using System.Diagnostics.Eventing.Reader;
using Translator.Core.Data;
using Translator.Desktop.Explorer.Graph;
using Translator.Desktop.Explorer.JSONItems;
using Translator.Desktop.Explorer.Story;
using Translator.Desktop.InterfaceImpls;
using Translator.Helpers;
using TabManager = Translator.Core.TabManager<Translator.Desktop.InterfaceImpls.WinLineItem, Translator.Desktop.InterfaceImpls.WinUIHandler, Translator.Desktop.InterfaceImpls.WinTabController, Translator.Desktop.InterfaceImpls.WinTab>;
using TranslationManager = Translator.Core.TranslationManager<Translator.Desktop.InterfaceImpls.WinLineItem, Translator.Desktop.InterfaceImpls.WinUIHandler, Translator.Desktop.InterfaceImpls.WinTabController, Translator.Desktop.InterfaceImpls.WinTab>;

namespace Translator.Desktop.Managers
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
            data = new();
            var explorer = new ContextProvider(new(), story == Path.GetFileNameWithoutExtension(path), false, filename, story, path);
            NodeList nodes = explorer.GetTemplateNodes();
            if (nodes != null)
            {
                data = new();
                if (story != filename) data.Add("Name", new LineData("Name", story, filename, StringCategory.General, filename, true));

                //Add all new lines, but check if they are relevant
                for (int i = 0; i < nodes.Count; i++)
                {
                    //filter out irrelevant nodes
                    if (nodes[i].ID == string.Empty) continue;
                    switch (nodes[i].Type)
                    {
                        case NodeType.Null:
                        case NodeType.BGCResponse:
                        case NodeType.CharacterGroup:
                        case NodeType.ItemGroup:
                        case NodeType.Criterion:
                        case NodeType.Pose:
                        case NodeType.Clothing:
                        case NodeType.CriteriaGroup:
                        case NodeType.Cutscene:
                        case NodeType.Door:
                        case NodeType.EventTrigger:
                        case NodeType.Property:
                        case NodeType.Personality:
                        case NodeType.Social:
                        case NodeType.State:
                        case NodeType.Value:
                            continue;
                        case NodeType.Item:
                        {
                            if (story == filename)
                                if (nodes[i].DataType == typeof(ItemOverride))
                                    data[((ItemOverride?)nodes[i].Data)?.DisplayName!] = new LineData(((ItemOverride?)nodes[i].Data)?.DisplayName!, story, filename, nodes[i].Type.CategoryFromNode(), ((ItemOverride?)nodes[i].Data)?.DisplayName!, true);
                                else if (nodes[i].Text != string.Empty && nodes[i].ID != string.Empty)
                                    data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].ID, true);
                            continue;
                        }
                        default:
                            break;
                    }
                    if (nodes[i].Type == NodeType.Event && nodes[i].DataType == typeof(GameEvent))
                    {
                        if (((GameEvent?)nodes[i].Data)?.EventType == StoryEnums.GameEvents.DisplayGameMessage)
                            data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), ((GameEvent?)nodes[i].Data)?.Value!, true);

                        else if (((GameEvent?)nodes[i].Data)?.EventType == StoryEnums.GameEvents.Item)
                            if (((GameEvent?)nodes[i].Data)?.Option == 2)
                                data[((GameEvent?)nodes[i].Data)?.Value!] = new LineData(((GameEvent?)nodes[i].Data)?.Value!, story, filename, nodes[i].Type.CategoryFromNode(), ((GameEvent?)nodes[i].Data)?.Value!, true);
                    }
                    else if (nodes[i].Type == NodeType.BGC)
                    {
                        data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].Text, true);
                    }
                    else
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
