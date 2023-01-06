using System.Timers;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;

namespace Translator.Core
{
    public interface ITranslationManager<out T> where T : class, ILineItem, new()
    {
        bool ChangesPending { get; set; }
        string CleanedSearchQuery { get; }
        string FileName { get; set; }
        string SearchQuery { get; }
        string SelectedId { get; }
        LineData SelectedLine { get; }
        T SelectedSearchResultItem { get; }
        string SourceFilePath { get; set; }
        string StoryName { get; set; }

        void ApprovedButtonHandler();
        void ApproveIfPossible(bool SelectNewAfter);
        bool CustomStoryTemplateHandle(string story);
        void LoadFileIntoProgram();
        void LoadFileIntoProgram(string path);
        void OverrideCloudSave();
        void PopulateTextBoxes();
        void ReloadFile();
        void ReloadTranslationTextbox();
        void ReplaceAll(string replacement);
        void ReplaceSingle(string replacement);
        void RequestedAutomaticTranslation();
        void SaveCurrentString();
        void SaveFile();
        void SaveFileAs();
        void SaveFileHandler(object? sender, ElapsedEventArgs? e);
        void Search();
        void Search(string query);
        void SelectLine(string id);
        bool SelectNextResultIfApplicable();
        string SelectSaveLocation();
        void ShowAutoSaveDialog();
        void ToggleReplaceUI();
        void UpdateCharacterCountLabel(int TranslationCount, int TemplateCount);
        void UpdateComments();
        void UpdateTranslationString();
    }
}