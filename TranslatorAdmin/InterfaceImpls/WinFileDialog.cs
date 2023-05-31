using System;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Desktop.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    public class WinFileDialog : IFileDialog
    {
        public WinFileDialog()
        {
            MultiSelect = false;
        }
        public WinFileDialog(string title, string filter, string initialDirectory, string fileName)
        {
            dialog.Multiselect = true;
            Title = title;
            Filter = filter;
            InitialDirectory = initialDirectory;
            FileName = fileName;
        }

        private readonly OpenFileDialog dialog = new();
        public string FileName { get => dialog.SafeFileName; set => Path.Combine(Path.GetDirectoryName(dialog.FileName.AsSpan()).ToString(), value); }
        public string Filter { get => dialog.Filter; set => dialog.Filter = value; }
        public string InitialDirectory { get => dialog.InitialDirectory; set => dialog.InitialDirectory = value; }
        public string SelectedPath => dialog.FileName;
        public string[] SelectedPaths => dialog.FileNames;
        public string Title { get => dialog.Title; set => dialog.Title = value; }
        public bool MultiSelect { get => dialog.Multiselect; set => dialog.Multiselect = value; }
        public bool CheckFileExists { get => dialog.CheckFileExists; set => dialog.CheckFileExists = value; }

        PopupResult IFileDialog.ShowDialog() => dialog.ShowDialog().ToPopupResult();
    }
}
