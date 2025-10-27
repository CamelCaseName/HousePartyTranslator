using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface IFileDialog
    {
        //constructer in this order
        string Title { get; set; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }
        string FileName { get; set; }

        //other settings
        bool MultiSelect { get; set; }
        bool CheckFileExists { get; set; }

        //returned values
        string SelectedPath { get; }
        string[] SelectedPaths { get; }
        PopupResult ShowDialog();
    }
}