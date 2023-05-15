using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface IFileDialog
    {
        bool MultiSelect { get; set; }
        string FileName { get; set; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }
        string SelectedPath { get; }
        string[] SelectedPaths { get; }
        string Title { get; set; }
        PopupResult ShowDialog();
    }
}