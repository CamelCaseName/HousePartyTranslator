using System;
using System.Collections.Generic;
using Translator.UICompatibilityLayer.StubImpls;

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

    public interface IFolderDialog
    {
        string SelectedFolderPath { get; set; }
        string Text { get; set; }
        PopupResult ShowDialog();
    }

    public interface ILineItem
    {
        public LineList Parent { get; init; }
        public bool IsApproved { get; set; }
        public bool IsSearchResult { get; set; }
        public bool IsTranslated { get; set; }
        EventHandler OnClick { get; init; }
        public string Text { get; init; }
        void Approve();
        void Unapprove();
    }

    public interface IMenuItem
    {
        EventHandler OnClick { get; init; }
        public string Text { get; set; }
    }

    public interface ITab
    {
        string Text { get; set; }
        bool IsApproveButtonFocused { get; }

        LineList Lines { get { return GetLines(); } }

        List<string> SimilarStringsToEnglish { get; }

        void Dispose();

        #region list of translations
        void ClearLines();

        ILineItem GetLineItem(int index);

        LineList GetLines();

        int SelectedLineIndex();
        ILineItem SelectedLineItem();
        void SelectLineItem(int index);

        void SelectLineItem(ILineItem item);

        void SetLines(LineList lines);
        void UpdateLines();

        #endregion

        #region translation textbox
        void FocusTranslationBox();

        string GetTranslationBoxText();

        string SelectedTranslationBoxText();

        void SetSelectedTranslationBoxText(int start, int end);

        void SetTranslationBoxText(string text);
        #endregion

        #region template textbox
        string GetTemplateBoxText();

        string SelectedTemplateBoxText();

        void SetSelectedTemplateBoxText(int start, int end);

        void SetTemplateBoxText(string text);
        #endregion

        #region comment textbox
        void FocusCommentBox();

        string GetCommentBoxText();

        string SelectedCommentBoxText();

        void SetCommentBoxText(string text);
        void SetSelectedCommentBoxText(int start, int end);
        #endregion

        #region line controls
        void ApproveSelectedLine();
        void UnapproveSelectedLine();
        bool GetApprovedButtonChecked();
        void SetApprovedButtonChecked(bool v);

        #endregion
    }

    public interface ITabController
    {
        int SelectedIndex { get; set; }
        ITab SelectedTab { get; set; }
        int TabCount { get; }
        List<ITab> TabPages { get; }
        bool CloseTab(ITab tab);
    }

    public interface ITextBox
    {
        public EventHandler OnClick { get; init; }
        public EventHandler OnTextChanged { get; init; }
        public int SelectionEnd { get; set; }
        public int SelectionStart { get; set; }
        public string Text { get; set; }
        public void Focus();
    }

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
        MenuItems GetFileMenuItems();

        void SetFileMenuItems(MenuItems menuItems);

        void SetTitle(string title);
        #endregion

        #region login user control
        bool Login();
        bool Logout();

        #endregion

        #region search/replace
        void FocusReplaceBar();

        void FocusSearchBar();
        string GetReplaceBarText();

        string GetSearchBarText();

        void SetReplaceBarText(string replacement);

        void SetSearchBarText(string query);

        void UpdateResults();
        #endregion

        #region menubar

        #endregion

        #region file access/system access
        Type FileDialogType { get; init; }

        Type FolderDialogType { get; init; }

        void ClipboardSetText(string text);

        ITab? CreateNewTab();

        void SignalAppExit();
        void Update();
        void UpdateProgress();
        #endregion

        #region tabs
        ITabController TabControl { get; }
        #endregion
    }

    public class LineList
    {
        public readonly List<ILineItem> Items = new();

        public int Count => Items.Count;
        public int ApprovedCount { get; internal set; }
        public LineList() : this(new List<ILineItem>()) { }

        public LineList(List<ILineItem> items, ILineItem selectedLineItem, int selectedIndex)
        {
            Items = items;
            SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public LineList(List<ILineItem> items)
        {
            Items = items;
            SelectedLineItem = items.Count > 0 ? items[0] : NullLineItem.Instance;
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public int SelectedIndex { get { return InternalSelectedIndex; } set { SelectIndex(value); } }
        public ILineItem SelectedLineItem { get; set; }
        private int InternalSelectedIndex { get; set; }

        public void AddLineItem(ILineItem item)
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

        public void RemoveLineItem(ILineItem item)
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
    }

    public class MenuItems
    {
        public readonly List<IMenuItem> Items = new();
        public int Count { get { return Items.Count; } }

        public IMenuItem this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }
        internal void Insert<MenuItemType>(int v, MenuItemType menuItem) where MenuItemType : class, IMenuItem
        {
            Items.Insert(v, menuItem);
        }
        internal void RemoveAt(int v)
        {
            Items.RemoveAt(v);
        }
    }
}