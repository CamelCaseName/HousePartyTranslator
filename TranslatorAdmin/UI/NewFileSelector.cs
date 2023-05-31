using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Translator.Core;

namespace Translator.Desktop.UI
{
    public partial class NewFileSelector : Form
    {
        public string CombinedStoryFile { get; private set; } = string.Empty;
        public string StoryName { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        private readonly HashSet<string> stories;
        private readonly Dictionary<string, List<string>> files;

        public NewFileSelector()
        {
            InitializeComponent();
            DataBase.GetAllFilesAndStoriesSorted(out stories, out files);
        }

        private void NewFileSelector_Load(object sender, EventArgs e)
        {
            storyDropdown.Items.Clear();
            storyDropdown.Items.AddRange(stories.ToArray());
        }

        private void Submit_click(object sender, EventArgs e)
        {
            if (storyDropdown.SelectedItem == null)
            {
                CombinedStoryFile = string.Empty;
                return;
            }
            StoryName = storyDropdown.SelectedItem.ToString()!;
            FileName = fileDropdown.SelectedItem.ToString()!;
            CombinedStoryFile = Path.Combine(StoryName, FileName);
            DialogResult = DialogResult.OK;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            CombinedStoryFile = string.Empty;
            DialogResult = DialogResult.Cancel;
        }

        private void StoryDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            fileDropdown.Items.Clear();
            fileDropdown.Items.AddRange(files[storyDropdown.SelectedItem.ToString()!].ToArray());
        }
    }
}
