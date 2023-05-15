using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface IFolderDialog
    {
        string SelectedFolderPath { get; set; }
        string Text { get; set; }
        PopupResult ShowDialog();
    }
}