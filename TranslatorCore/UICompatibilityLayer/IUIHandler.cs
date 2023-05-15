using System;
using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface IUIHandler<TLineItem, TTabController, TTab>
        where TLineItem : class, ILineItem, new()
        where TTabController : class, ITabController<TLineItem, TTab>, new()
        where TTab : class, ITab<TLineItem>, new()
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

        void UpdateResults();
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

        TTab? CreateNewTab();

        void SignalAppExit();
        void Update();
        void UpdateTranslationProgressIndicator();
        void SetReplaceMenuVisible();
        void SetReplaceMenuInVisible();
        void SetFileMenuItems(MenuItems menuItems);
        void SetSelectedSearchBarText(int v, int length);
        #endregion

        #region tabs
        TTabController TabControl { get; }
        string Language { get; set; }
        bool ReplaceBarIsVisible { get; }
        #endregion

        #region counts
        int TranslationBoxTextLength { get; }
        int TemplateBoxTextLength { get; }
        int TemplateBoxSelectedTextLength { get; }
        int TranslationBoxSelectedTextLength { get; }
        TTab SelectedTab { get; }
        string TranslationBoxText { get; set; }
        string TemplateBoxText { get; set; }
        #endregion
    }
}