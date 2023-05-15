﻿using System.Timers;
using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ITranslationManager<out TLineItem> where TLineItem : class, ILineItem, new()
    {
        bool ChangesPending { get; set; }
        string CleanedSearchQuery { get; }
        string FileName { get; set; }
        string SearchQuery { get; }
        string SelectedId { get; }
        LineData SelectedLine { get; }
        TLineItem SelectedSearchResultItem { get; }
        string SourceFilePath { get; set; }
        string StoryName { get; set; }

        void ApprovedButtonHandler();
        void ApproveIfPossible(bool SelectNewAfter);
        bool CustomStoryTemplateHandle(string story);
        void CreateTemplateForSingleFile();
        void CreateTemplateForAllFiles();
        void LoadFileIntoProgram(string path);
        void OverrideCloudSave();
        void PopulateTextBoxes();
        void ReloadFile();
        void ReloadTranslationTextbox();
        void ReplaceAll(string replacement);
        void ReplaceSingle(string replacement);
        void RequestAutomaticTranslation();
        void SaveCurrentString();
        void SaveFile(bool doOnlineUpdate = true);
        void SaveFileAs();
        void SaveFileHandler(object? sender, ElapsedEventArgs? e);
        void Search();
        void Search(string query);
        void SelectLine(string id);
        bool SelectNextResultIfApplicable();
        string SelectSaveLocation(string file = "", string path = "");
        void ToggleReplaceUI();
        void UpdateCharacterCountLabel(int TranslationCount, int TemplateCount);
        void UpdateComments();
        void UpdateTranslationString();
    }
}