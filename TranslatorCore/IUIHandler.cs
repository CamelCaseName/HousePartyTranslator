using System;
using System.Collections.Generic;

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

    public class MenuItems
    {
        public readonly List<IMenuItem> Items = new();
        public IMenuItem this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }
        public int Count { get { return Items.Count; } }

        internal void Insert<MenuItemType>(int v, MenuItemType menuItem) where MenuItemType : class, IMenuItem {
            Items.Insert(v, menuItem);
        }
        internal void RemoveAt(int v)
        {
            Items.RemoveAt(v);
        }
    }

    public interface IMenuItem
    {
        public string Text { get; set; }
        EventHandler OnClick { get; init; }
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

    public class LineList
    {
        public readonly List<ILineItem> Items = new();
        public ILineItem SelectedLineItem { get; set; }
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
            SelectedLineItem = items.Count > 0 ? items[0] : new NullLineItem();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }
        public void ApproveItem(int index)
        {
            try
            {
                Items[index].Approve();
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }
        public void Clear()
        {
            Items.Clear();
        }
        public void AddLineItem(ILineItem item)
        {
            Items.Add(item);
        }
        public void RemoveLineItem(ILineItem item)
        {
            Items.Remove(item);
        }
        public void UnapproveItem(int index)
        {
            try
            {
                Items[index].Unapprove();
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
                Items[index].IsApproved = isApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
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
        private int _publicSelectedIndex { get; set; }
        public int SelectedIndex { get { return _publicSelectedIndex; } set { SelectIndex(value); } }
        public void SelectIndex(int index)
        {
            try
            {
                _publicSelectedIndex = index;
                SelectedLineItem = Items[index];
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }
    }

    public interface ILineItem
    {
        public string Text { get; init; }
        public bool IsApproved { get; set; }
        public bool IsTranslated { get; set; }
        public bool IsSearchResult { get; set; }
        EventHandler OnClick { get; init; }
        void Approve();
        void Unapprove();
    }

    public class NullLineItem : ILineItem
    {
        public string Text { get => string.Empty; init { } }
        public bool IsApproved { get => false; set { } }
        public bool IsTranslated { get => false; set { } }
        public bool IsSearchResult { get => false; set { } }
        public EventHandler OnClick { get { return new((object? sender, EventArgs e) => { }); } init { } }

        public void Approve() { }
        public void Unapprove() { }
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
        void Update();
        #endregion

        #region tabs

        #endregion
    }
}
