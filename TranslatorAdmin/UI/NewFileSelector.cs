using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.Foundation;

namespace Translator.Desktop.UI
{
    [SupportedOSPlatform("Windows")]
    public partial class NewFileSelector : Form, INewFileSelector
    {
        public string CombinedStoryFile { get; private set; } = string.Empty;
        public string StoryName { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public readonly HashSet<string> stories;
        public readonly Dictionary<string, List<string>> files;
        private readonly bool storyOnly;

        public NewFileSelector(bool StoryOnly = false)
        {
            InitializeComponent();
            DataBase.GetAllFilesAndStoriesSorted(out stories, out files);
            storyOnly = StoryOnly;
            if (storyOnly)
            {
                fileDropdown.Enabled = false;
                fileDropdown.Visible = false;
                fileLabel.Visible = false;
            }
        }

        private void NewFileSelector_Load(object sender, EventArgs e)
        {
            storyDropdown.Items.Clear();
            object[] objects = new object[stories.Count];
            HashSet<string>.Enumerator enumerator = stories.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                objects[i++] = enumerator.Current;
            }
            storyDropdown.Items.AddRange(objects);
        }

        private void Submit_click(object sender, EventArgs e)
        {
            if (storyDropdown.SelectedItem is null)
            {
                CombinedStoryFile = string.Empty;
                return;
            }
            StoryName = storyDropdown.SelectedItem.ToString()!;
            FileName = fileDropdown.SelectedItem?.ToString() ?? string.Empty;
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
            if (storyOnly)
            {
                submit.Enabled = true;
                return;
            }
            fileDropdown.Items.Clear();
            fileDropdown.Items.AddRange(files[storyDropdown.SelectedItem.ToString()!].ToArray());
        }

        private void FileDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            submit.Enabled = true;
        }

        public new PopupResult ShowDialog()
        {
            return base.ShowDialog().ToPopupResult();
        }
    }
}
