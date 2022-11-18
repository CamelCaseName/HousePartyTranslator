using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UICompatibilityLayer
{
    public enum PopupResult
    {
        NONE,
        OK,
        YES,
        NO,
        CANCEL,
        ABORT,
        CONTINUE,
        IGNORE
    }

    public class MenuItems
    {
        public readonly List<IMenuItem> Items = new();
    }

    public interface IMenuItem
    {
        public string Text { get; set; }
        EventHandler OnClick { get; init; }
    }

    public class LineList
    {
        public readonly List<ILineItem> Items = new();
    }

    public interface ILineItem
    {
        public string Text { get; init; }
        public bool IsApproved { get; }
        public bool IsTranslated { get; set; }
        public bool IsSearchResult { get; set; }
        EventHandler OnClick { get; init; }
        void Approve();
        void Unapprove();
    }

    public interface IUIHandler
    {
        #region cursor
        void SignalUserWait();
        void SignalUserEndWait();

        #endregion

        #region message popup
        PopupResult InfoOk(string message, string title = "Info");
        PopupResult InfoOkCancel(string message, string title = "Info");
        PopupResult InfoYesNo(string message, string title = "Info");
        PopupResult InfoYesNoCancel(string message, string title = "Info");
        PopupResult WarningOk(string message, string title = "Warning");
        PopupResult WarningOkCancel(string message, string title = "Warning");
        PopupResult WarningYesNo(string message, string title = "Warning");
        PopupResult WarningYesNoCancel(string message, string title = "Warning");
        PopupResult ErrorOk(string message, string title = "Error");
        PopupResult ErrorOkCancel(string message, string title = "Error");
        PopupResult ErrorYesNo(string message, string title = "Error");
        PopupResult ErrorYesNoCancel(string message, string title = "Error");
        bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK);
        bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES);
        bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES);
        bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK);
        bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES);
        bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES);
        bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK);
        bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES);
        bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES);

        #endregion

        #region main window
        void SetTitle(string title);
        MenuItems GetFileMenuItems();
        void SetFileMenuItems(MenuItems menuItems);

        #endregion

        #region list of translations
        int SelectedLineIndex();
        ILineItem SelectedLineItem();
        LineList GetLines();
        void SetLines(LineList lines);
        ILineItem GetLineItem(int index);
        void SelectLineItem(int index);
        void SelectLineItem(ILineItem item);
        void ClearLines();
        void UpdateLines();

        #endregion

        #region translation textbox
        void SetTranslationBoxText(string text);
        string GetTranslationBoxText();
        string SelectedTranslationBoxText();
        void SetSelectedTranslationBoxText(int start, int end);
        void FocusTranslationBox();

        #endregion

        #region template textbox
        void SetTemplateBoxText(string text);
        string GetTemplateBoxText();
        string SelectedTemplateBoxText();
        void SetSelectedTemplateBoxText(int start, int end);

        #endregion

        #region comment textbox
        void SetCommentBoxText(string text);
        string GetCommentBoxText();
        string SelectedCommentBoxText();
        void SetSelectedCommentBoxText(int start, int end);
        void FocusCommentBox();

        #endregion

        #region line controls
        void ApproveSelectedLine();
        void UnapproveSelectedLine();

        #endregion

        #region login user control
        bool Login();
        bool Logout();

        #endregion

        #region search/replace
        void FocusSearchBar();
        void UpdateResults();
        string GetSearchBarText();
        void SetSearchBarText(string query);
        void FocusReplaceBar();
        string GetReplaceBarText();
        void SetReplaceBarText(string replacement);

        #endregion

        #region menubar

        #endregion

        #region file access/system access
        void SignalAppExit();
        #endregion

        #region tabs

        #endregion
    }
}
