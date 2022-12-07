using Translator;
using Translator.Core.Helpers;
using Translator.Helpers;
using TranslatorAdmin.InterfaceImpls;

namespace TranslatorAdmin.Managers
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class WinTranslationManager : Translator.Core.TranslationManager<WinLineItem>
    {
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

    }
}
