using Translator.Helpers;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinFolderDialog : IFolderDialog
    {
        private readonly FolderBrowserDialog dialog = new() { UseDescriptionForTitle = true , RootFolder = Environment.SpecialFolder.Recent};
        public string SelectedFolderPath { get => dialog.SelectedPath; set => dialog.SelectedPath = value; }
        public string Text { get => dialog.Description; set => dialog.Description = value; }

        public PopupResult ShowDialog() => dialog.ShowDialog().ToPopupResult();
    }
}
