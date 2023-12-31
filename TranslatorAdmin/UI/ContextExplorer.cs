using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Desktop.Managers;
using Translator.Explorer.Graph;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    public sealed partial class ContextExplorer : Form
    {
        private readonly string FileName, StoryName;
        public ContextExplorer(string storyName, string fileName)
        {
            FileName = fileName;
            StoryName = storyName;
            InitializeComponent();
        }

        public void SetLines(Node current)
        {
            Lines.Nodes.Clear();
            Lines.Nodes.Add("Text leading up to this dialogue/affecting it: ");
            foreach (var parent in current.ParentNodes)
            {
                if (!WinTranslationManager.TryExtractTemplateText(StoryName, FileName, parent, out var template))
                    continue;

                if (template is null)
                    continue;

                Lines.Nodes[0].Nodes.Add(template.ToString());
            }
            Lines.Nodes.Add(current.ToOutputFormat());
            Lines.Nodes.Add("Leads to/affects: ");
            foreach (var child in current.ChildNodes)
            {
                if (!WinTranslationManager.TryExtractTemplateText(StoryName, FileName, child, out var template))
                    continue;

                if (template is null)
                    continue;

                Lines.Nodes[2].Nodes.Add(template.ToString());
            }
        }
    }
}
