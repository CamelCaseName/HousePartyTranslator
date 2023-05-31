using System.IO;
using System.Windows.Forms;
using Translator.Core;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Desktop.Explorer.Graph;
using Translator.Desktop.Explorer.JSONItems;
using Translator.Desktop.Explorer.Story;
using Translator.Desktop.InterfaceImpls;
using Translator.Desktop.UI;
using Translator.Helpers;
namespace Translator.Desktop.Managers
{
    //also contains some extensions to ease programming
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class WinTranslationManager
    {
        public static bool UpdateStoryExplorerSelection { get; internal set; } = true;

        public static DiscordPresenceManager? DiscordPresence { get; internal set; }

        internal static bool CreateTemplateFromStory(string story, string filename, string path, out FileData data)
        {
            if (TabManager.UI == null)
            {
                data = new(string.Empty, string.Empty);
                return false;
            }

            TabManager.UI.SignalUserWait();
            data = new(story, filename);
            var explorer = new ContextProvider(new(), story == filename, false, filename, story, path);
            NodeList nodes = explorer.GetTemplateNodes();
            if (nodes != null)
            {
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
                            if (story != filename) continue;

                            if (nodes[i].DataType == typeof(ItemOverride) && nodes[i].Data != null)
                            {
                                ItemOverride itemOverride = (ItemOverride)nodes[i].Data!;
                                data[itemOverride.DisplayName!] = new LineData(itemOverride.DisplayName!, story, filename, nodes[i].Type.CategoryFromNode(), itemOverride.DisplayName!, true);
                            }
                            else if (nodes[i].Text != string.Empty && nodes[i].ID != string.Empty)
                            {
                                data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].ID, true);
                            }
                            continue;
                        }
                        case NodeType.Event:
                        {
                            if (nodes[i].DataType != typeof(GameEvent) || nodes[i].Data == null) continue;

                            GameEvent gameEvent = (GameEvent)nodes[i].Data!;
                            if (gameEvent.EventType == StoryEnums.GameEvents.DisplayGameMessage)
                            {
                                data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), gameEvent.Value!, true);
                            }
                            else if (gameEvent.EventType == StoryEnums.GameEvents.Item)
                            {
                                if (gameEvent.Option == 2)
                                    data[gameEvent.Value!] = new LineData(gameEvent.Value!, story, filename, nodes[i].Type.CategoryFromNode(), gameEvent.Value!, true);
                            }
                            continue;
                        }
                        case NodeType.Achievement:
                        {
                            if (nodes[i].ID.Contains("SteamName")) continue;

                            data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].Text, true);
                            continue;
                        }
                        default:
                        {
                            data[nodes[i].ID] = new LineData(nodes[i].ID, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].Text, true);
                            continue;
                        }
                    }
                }

                TabManager.UI.SignalUserEndWait();

                return true;
            }

            _ = Msg.ErrorOk("Something broke, please try again.");
            TabManager.UI.SignalUserEndWait();
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
