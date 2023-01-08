using System;
using System.Collections.Generic;
using System.Drawing;
using Translator.Core;

namespace Translator.UICompatibilityLayer
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

    public interface IFileDialog
    {
        string FileName { get; set; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }
        string SelectedPath { get; }
        string Title { get; set; }
        PopupResult ShowDialog();
    }

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

    public interface IFolderDialog
    {
        string SelectedFolderPath { get; set; }
        string Text { get; set; }
        PopupResult ShowDialog();
    }

    public interface ILineItem
    {
        public bool IsApproved { get; set; }
        public bool IsSearchResult { get; set; }
        public bool IsTranslated { get; set; }
        public string Text { get; init; }
        void Approve();
        void Unapprove();
    }

    public interface IMenuItem
    {
        public event EventHandler Click { add { } remove { } }
        public string Text { get; set; }
    }

    public interface ITab<TLineItem>
        where TLineItem : class, ILineItem, new()
    {
        int LineCount => Lines.Count;
        string Text { get; set; }
        bool IsApproveButtonFocused { get; }

        /// <summary>
        /// Contains the ids of strings similar to the original template
        /// </summary>
        List<string> SimilarStringsToEnglish { get; }

        void Dispose();

        #region list of translations
        void ClearLines();

        TLineItem AtIndex(int index);

        ILineList<TLineItem> Lines { get; set; }
        int SelectedLineIndex { get; }
        TLineItem SelectedLineItem { get; }
        string SelectedLine { get { return SelectedLineItem.Text; } }

        bool IsTranslationBoxFocused { get; }
        bool IsCommentBoxFocused { get; }
        int ProgressValue { get; set; }

        void SelectLineItem(int index);

        void SelectLineItem(TLineItem item);
        void UpdateLines();

        #endregion

        #region translation textbox
        void FocusTranslationBox();

        string TranslationBoxText { get; set; }

        string SelectedTranslationBoxText { get; }

        void SetSelectedTranslationBoxText(int start, int end);
        #endregion

        #region template textbox
        string TemplateBoxText { get; set; }

        string SelectedTemplateBoxText { get; }

        void SetSelectedTemplateBoxText(int start, int end);
        #endregion

        #region comment textbox
        void FocusCommentBox();

        string CommentBoxText { get; set; }

        string[] CommentBoxTextArr { get; set; }

        string SelectedCommentBoxText();
        void SetSelectedCommentBoxText(int start, int end);
        #endregion

        #region line controls
        void ApproveSelectedLine();
        void UnapproveSelectedLine();
        bool ApprovedButtonChecked { get; set; }

        void SetFileInfoText(string info);
        void SetApprovedLabelText(string text);
        void SetCharacterLabelColor(Color color);
        void SetCharacterCountLabelText(string text);
        #endregion
    }

    public interface ITabController<TLineItem, TTab>
        where TLineItem : class, ILineItem, new()
        where TTab : class, ITab<TLineItem>, new()
    {
        int SelectedIndex { get; set; }
        TTab SelectedTab { get; set; }
        int TabCount { get; }
        List<TTab> TabPages { get; }
        void AddTab(TTab tab);
        bool CloseTab(TTab tab);
    }

    public interface ITextBox
    {
        public event EventHandler Click { add { } remove { } }
        public event EventHandler TextChanged { add { } remove { } }
        public int SelectionEnd { get; set; }
        public int SelectionStart { get; set; }
        public string Text { get; set; }
        public void Focus();
    }

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

    public interface ILineList<TLineItem>
        where TLineItem : class, ILineItem, new()
    {
        TLineItem this[int index] { get; set; }
        int ApprovedCount { get; }
        int Count { get; }
        List<string> SearchResults { get; }
        int SelectedIndex { get; set; }
        TLineItem SelectedLineItem { get; set; }
        List<string> TranslationSimilarToTemplate { get; }

        void Add(string iD, bool lineIsApproved);
        void AddLineItem(TLineItem item);
        void ApproveItem(int index);
        void Clear();
        void FreezeLayout();
        bool GetApprovalState(int index);
        void RemoveLineItem(TLineItem item);
        void SelectIndex(int index);
        void SetApprovalState(int index, bool isApproved);
        void UnapproveItem(int index);
        void UnFreezeLayout();
    }

    public class NullLineList<TLineItem> : ILineList<TLineItem>
        where TLineItem : class, ILineItem, new()
    {
        public readonly List<TLineItem> Items = new();

        public int Count => Items.Count;
        public int ApprovedCount { get; internal set; }
        public NullLineList() : this(new List<TLineItem>()) { }

        public NullLineList(List<TLineItem> items, TLineItem selectedLineItem, int selectedIndex)
        {
            Items = items;
            SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public NullLineList(List<TLineItem> items)
        {
            Items = items;
            SelectedLineItem = items.Count > 0 ? items[0] : new TLineItem();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public TLineItem this[int index] { get { return Items[index]; } set { Items[index] = value; } }

        public int SelectedIndex { get { return InternalSelectedIndex; } set { SelectIndex(value); } }
        public TLineItem SelectedLineItem { get; set; }
        private int InternalSelectedIndex { get; set; }
        public List<string> SearchResults { get; internal set; } = new();
        public List<string> TranslationSimilarToTemplate { get; internal set; } = new();

        public void AddLineItem(TLineItem item)
        {
            Items.Add(item);
        }

        public void ApproveItem(int index)
        {
            try
            {
                Items[index].Approve();
                ++ApprovedCount;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Clear()
        {
            Items.Clear();
            ApprovedCount = 0;
        }

        public bool GetApprovalState(int index)
        {
            try
            {
                return Items[index].IsApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void RemoveLineItem(TLineItem item)
        {
            if (item.IsApproved) --ApprovedCount;
            Items.Remove(item);
        }

        public void SelectIndex(int index)
        {
            try
            {
                InternalSelectedIndex = index;
                SelectedLineItem = Items[index];
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void SetApprovalState(int index, bool isApproved)
        {
            try
            {
                if (!Items[index].IsApproved && isApproved) ++ApprovedCount;
                Items[index].IsApproved = isApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void UnapproveItem(int index)
        {
            try
            {
                Items[index].Unapprove();
                --ApprovedCount;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Add(string iD, bool lineIsApproved)
        {
            Items.Add(new TLineItem() { Text = iD, IsApproved = lineIsApproved });
        }

        public void FreezeLayout() { }
        public void UnFreezeLayout() { }
    }

    public class MenuItems : List<IMenuItem>
    {
        public MenuItems() : base() { }

        public MenuItems(int capacity) : base(capacity) { }
    }
}