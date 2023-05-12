using System.Runtime.Versioning;
using Translator.UICompatibilityLayer;

namespace Translator.InterfaceImpls
{
    [SupportedOSPlatform("Windows")]
    public class WinSaveFileDialog : ISaveFileDialog
    {
        public WinSaveFileDialog() { }
        public WinSaveFileDialog(string title, string extension, bool createPrompt, bool overwritePromt, string fileName, string initialDirectory)
        {
            Title = title;
            Extension = extension;
            PromptCreate = createPrompt;
            PromptOverwrite = overwritePromt;
            FileName = fileName;
            InitialDirectory = initialDirectory;
        }
        private readonly SaveFileDialog dialog = new();
        public string FileName { get => dialog.FileName; set => dialog.FileName = value; }
        public string InitialDirectory { get => dialog.InitialDirectory; set => dialog.InitialDirectory = value; }
        public string Title { get => dialog.Title; set => dialog.Title = value; }
        public string Extension { get => dialog.DefaultExt; set => dialog.DefaultExt = value; }
        public bool PromptCreate { get => dialog.CreatePrompt; set => dialog.CreatePrompt = value; }
        public bool PromptOverwrite { get => dialog.OverwritePrompt; set => dialog.OverwritePrompt = value; }

        public PopupResult ShowDialog() => dialog.ShowDialog().ToPopupResult();
    }
}
