using System;
using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface IUIHandler
    {
        #region cursor
        void SignalUserEndWait();

        void SignalUserWait();
        #endregion

        #region message popup
        PopupResult ErrorOk(string message, string title = "Error");

        PopupResult ErrorOkCancel(string message, string title = "Error");

        bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK);

        PopupResult ErrorYesNo(string message, string title = "Error");

        bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES);

        PopupResult ErrorYesNoCancel(string message, string title = "Error");

        bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES);

        PopupResult InfoOk(string message, string title = "Info");
        PopupResult InfoOkCancel(string message, string title = "Info");
        bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK);

        PopupResult InfoYesNo(string message, string title = "Info");
        bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES);

        PopupResult InfoYesNoCancel(string message, string title = "Info");
        bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES);

        PopupResult WarningOk(string message, string title = "Warning");
        PopupResult WarningOkCancel(string message, string title = "Warning");
        bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK);

        PopupResult WarningYesNo(string message, string title = "Warning");
        bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES);

        PopupResult WarningYesNoCancel(string message, string title = "Warning");
        bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES);
        #endregion

        #region main window

        MenuItems FileMenuItems { get; }

        void SetTitle(string title);
        #endregion

        #region login user control
        bool Login();
        bool Logout();

        #endregion

        #region search/replace
        void FocusReplaceBar();

        void FocusSearchBar();
        string ReplaceBarText { get; set; }

        string SearchBarText { get; set; }

        int SearchResultCount { get; set; }
        int SelectedSearchResult { get; set; }

        void ToggleReplaceBar();
        void SetSelectedSearchBarText(int v, int length);
        #endregion

        #region menubar

        #endregion

        #region file access/system access
        Type? InternalFileDialogType { get; }
        Type FileDialogType { get => InternalFileDialogType ?? typeof(IFileDialog); }
        Type? InternalFolderDialogType { get; }
        Type FolderDialogType { get => InternalFolderDialogType ?? typeof(IFolderDialog); }
        Type? InternalSaveFileDialogType { get; }
        Type SaveFileDialogType { get => InternalSaveFileDialogType ?? typeof(ISaveFileDialog); }

        CreateTemplateFromStoryDelegate CreateTemplateFromStory { get; }

        void ClipboardSetText(string text);

        ITab? CreateNewTab();

        void SignalAppExit();
        void Update();
        void UpdateTranslationProgressIndicator();
        void SetReplaceMenuVisible();
        void SetReplaceMenuInVisible();
        void SignalUserPing();
        void SetLanguageHighlights(string[] languages);
        #endregion

        #region tabs
        ITabController TabControl { get; }
        string Language { get; set; }
        bool ReplaceBarIsVisible { get; }
        #endregion

        #region counts
        int TranslationBoxTextLength { get; }
        int TemplateBoxTextLength { get; }
        int TemplateBoxSelectedTextLength { get; }
        int TranslationBoxSelectedTextLength { get; }
        ITab SelectedTab { get; }
        string TranslationBoxText { get; set; }
        string TemplateBoxText { get; set; }
        #endregion
    }
}