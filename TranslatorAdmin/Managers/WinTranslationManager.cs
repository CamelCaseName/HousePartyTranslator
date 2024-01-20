using Translator.Core;
using Translator.Core.Data;
using Translator.Explorer.Graph;
using Translator.Explorer.JSONItems;
using Translator.Explorer.Story;
using Translator.Desktop.InterfaceImpls;
using Translator.Helpers;

namespace Translator.Desktop.Managers
{
    //also contains some extensions to ease programming
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class WinTranslationManager
    {
        public static bool CreateTemplateFromStory(string story, string filename, string path, out FileData data)
        {
            if (TabManager.UI is null)
            {
                data = new(string.Empty, string.Empty);
                return false;
            }

            TabManager.UI.SignalUserWait();
            data = new(story, filename);
            var contextProvider = new ContextProvider(new(), story == filename, false, filename, story, path);
            NodeList nodes = contextProvider.GetTemplateNodes();
            if (nodes is not null)
            {
                if (story != filename) data.Add(new("Name", StringCategory.General), new LineData("Name", story, filename, StringCategory.General, filename, true));

                //Add all new lines, but check if they are relevant
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (TryExtractTemplateText(story, filename, nodes[i], out var template))
                        if (template is not null)
                            data[template.EekID] = template;
                }
                TabManager.UI.SignalUserEndWait();

                return true;
            }

            _ = Msg.ErrorOk("Something broke, please try again.");
            TabManager.UI.SignalUserEndWait();
            return false;
        }

        public static bool TryExtractTemplateText(string story, string filename, Node node, out LineData? template)
        {
            template = null;
            //filter out irrelevant nodes
            if (node.ID == string.Empty) return false;
            switch (node.Type)
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
                    return false;
                case NodeType.Item:
                {
                    if (story != filename) return false;

                    if (node.DataType == typeof(ItemOverride) && node.Data is not null)
                    {
                        ItemOverride itemOverride = (ItemOverride)node.Data!;
                        template = new LineData(itemOverride.DisplayName!, story, filename, node.Type.CategoryFromNode(), itemOverride.DisplayName!, true);
                    }
                    else if (node.DataType == typeof(UseWith) && node.Data is not null)
                    {
                        UseWith use = (UseWith)node.Data!;
                        if (use.CustomCantDoThatMessage != string.Empty)
                            //not sure if this can even work but ill try, maybe we need the english version as id?
                            template = new LineData(use.ItemName! + "CustomCantDoThatMessage", story, filename, node.Type.CategoryFromNode(), use.CustomCantDoThatMessage!, true);
                    }
                    else if (node.Text != string.Empty && node.ID != string.Empty)
                    {
                        template = new LineData(node.Text, story, filename, node.Type.CategoryFromNode(), node.Text, true);
                    }
                    return true;
                }
                case NodeType.Event:
                {
                    if (node.DataType != typeof(GameEvent) || node.Data is null) return false;

                    GameEvent gameEvent = (GameEvent)node.Data!;
                    if (gameEvent.EventType == StoryEnums.GameEvents.DisplayGameMessage)
                    {
                        template = new LineData(node.ID, story, filename, node.Type.CategoryFromNode(), gameEvent.Value!, true);
                    }
                    else if (gameEvent.EventType == StoryEnums.GameEvents.Item)
                    {
                        if (gameEvent.Option == 2)
                            template = new LineData(gameEvent.Value!, story, filename, node.Type.CategoryFromNode(), gameEvent.Value!, true);
                    }
                    return true;
                }
                case NodeType.Achievement:
                {
                    if (node.ID.Contains("SteamName")) return false;

                    template = new LineData(node.ID, story, filename, node.Type.CategoryFromNode(), node.Text, true);
                    return true;
                }
                default:
                {
                    template = new LineData(node.ID, story, filename, node.Type.CategoryFromNode(), node.Text, true);
                    return true;
                }
            }
        }

        /// <summary>
        /// Sets the node whose tree gets highlighted to the one representing the currently selected string;
        /// </summary>
        public static void SetHighlightedNode(this TranslationManager manager)
        {
            if (TabManager.UI is null) return;
            if (manager.TranslationData.Count > 0)
            {
                //Highlights the node representign the selected string in the story explorer window
                if (App.MainForm?.Explorer is not null && !App.MainForm.Explorer.IsDisposed)
                {
                    App.MainForm.Invoke(() => App.MainForm.Explorer.Grapher.HighlightedNode = App.MainForm.Explorer?.Provider.Nodes.Find(n => n.ID == manager.SelectedId && n.FileName == manager.FileName) ?? Node.NullNode);
                }
            }
        }
    }
}
