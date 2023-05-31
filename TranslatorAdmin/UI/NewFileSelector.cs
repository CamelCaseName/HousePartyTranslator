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
                FileName = string.Empty;
                return;
            }
            FileName = Path.Combine(storyDropdown.SelectedItem.ToString()!, string.Concat(fileDropdown.SelectedText, ".txt"));
            DialogResult = DialogResult.OK;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            FileName = string.Empty;
            DialogResult = DialogResult.Cancel;
        }

        private void StoryDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FileDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
