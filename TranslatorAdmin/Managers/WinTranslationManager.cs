using Translator;
using Translator.Core;
using Translator.Core.Helpers;
using Translator.Helpers;
using TranslatorAdmin.InterfaceImpls;

namespace TranslatorAdmin.Managers
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class WinTranslationManager : Translator.Core.TranslationManager<WinLineItem>
    {
        public bool UpdateStoryExplorerSelection = true;
        private readonly WinUIHandler UI;
        internal WinTranslationManager(WinUIHandler ui) : base(ui, ui.TabControl.SelectedTab)
        {
            UI ??= ui;
        }
        internal bool CreateTemplateFromStory(string story, string filename, string path, out FileData data)
        {
            UI.SignalUserWait();
            data = new FileData();
            var explorer = new ContextProvider(story == Path.GetFileNameWithoutExtension(path), false, filename, story, null);
            List<Node> nodes = explorer.GetTemplateNodes();
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
                    if (!(
                            (int.TryParse(nodes[i].Text, out _) || nodes[i].Text.Length < 2)
                            && nodes[i].Type == NodeType.Event)
                    && nodes[i].Type.CategoryFromNode() != StringCategory.Neither
                        && nodes[i].ID != "")
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
        internal void SetHighlightedNode()
        {
            if (TranslationData.Count > 0)
            {
                int currentIndex = helper.CheckListBoxLeft.SelectedIndex;
                string id = currentIndex < TranslationData.Count && currentIndex >= 0 ? TranslationData[SelectedId].ID : "Name";
                //Highlights the node representign the selected string in the story explorer window
                if (MainWindow.Explorer != null && !MainWindow.Explorer.IsDisposed)
                {
                    MainWindow.Explorer.Grapher.HighlightedNode = MainWindow.Explorer.Grapher.Context.Nodes.Find(n => n.ID == id);
                }
            }
        }
    }
}
