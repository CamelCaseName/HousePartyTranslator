using System.Runtime.Versioning;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    public class WinFolderDialog : IFolderDialog
    {
        public WinFolderDialog() { }

        public WinFolderDialog(string text, string selectedPath)
        {
            Text = text;
            SelectedFolderPath = selectedPath;
        }

        private readonly FolderBrowserDialog dialog = new() { UseDescriptionForTitle = true, RootFolder = Environment.SpecialFolder.Recent };
        public string SelectedFolderPath { get => dialog.SelectedPath; set => dialog.SelectedPath = value; }
        public string Text { get => dialog.Description; set => dialog.Description = value; }

        public PopupResult ShowDialog() => dialog.ShowDialog().ToPopupResult();
    }
}
