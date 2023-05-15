using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ISaveFileDialog
    {
        string FileName { get; set; }
        string InitialDirectory { get; set; }
        string Title { get; set; }
        string Extension { get; set; }
        bool PromptCreate { get; set; }
        bool PromptOverwrite { get; set; }

        PopupResult ShowDialog();
    }
}