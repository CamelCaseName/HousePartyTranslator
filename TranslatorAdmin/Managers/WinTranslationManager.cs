using Translator.Core;
using Translator.Core.Data;
using Translator.Desktop.Explorer.Graph;
using Translator.Desktop.Explorer.JSONItems;
using Translator.Desktop.Explorer.Story;
using Translator.Desktop.InterfaceImpls;
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
                        case NodeType.BGCResponse:
                        case NodeType.CharacterGroup:
                        case NodeType.Clothing:
                        case NodeType.CriteriaGroup:
                        case NodeType.Criterion:
                        case NodeType.Cutscene:
                        case NodeType.Door:
                        case NodeType.EventTrigger:
                        case NodeType.ItemGroup:
                        case NodeType.Null:
                        case NodeType.Personality:
                        case NodeType.Pose:
                        case NodeType.Property:
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
                            else if (nodes[i].DataType == typeof(UseWith) && nodes[i].Data != null)
                            {
                                UseWith use = (UseWith)nodes[i].Data!;
                                if (use.CustomCantDoThatMessage != string.Empty)
                                    //not sure if this can even work but ill try, maybe we need the english version as id?
                                    data[use.ItemName! + "CustomCantDoThatMessage"] = new LineData(use.ItemName! + "CustomCantDoThatMessage", story, filename, nodes[i].Type.CategoryFromNode(), use.CustomCantDoThatMessage!, true);
                            }
                            else if (nodes[i].Text != string.Empty && nodes[i].ID != string.Empty)
                            {
                                data[nodes[i].Text] = new LineData(nodes[i].Text, story, filename, nodes[i].Type.CategoryFromNode(), nodes[i].Text, true);
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
                //Highlights the node representign the selected string in the story explorer window
                if (App.MainForm?.Explorer != null && !App.MainForm.Explorer.IsDisposed)
                {
                    App.MainForm.Invoke(() => App.MainForm.Explorer.Grapher.HighlightedNode = App.MainForm.Explorer?.Provider.Nodes.Find(n => n.ID == manager.SelectedId && n.FileName == manager.FileName) ?? Node.NullNode);
                }
            }
        }
    }
}
