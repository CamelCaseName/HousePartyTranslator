using System.Runtime.Versioning;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    public class WinFileDialog : IFileDialog
    {
        public WinFileDialog() { }
        public WinFileDialog(string title, string filter, string initialDirectory, string fileName) {
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
        public string Title { get => dialog.Title; set => dialog.Title = value; }

        PopupResult IFileDialog.ShowDialog() => dialog.ShowDialog().ToPopupResult();
    }
}
