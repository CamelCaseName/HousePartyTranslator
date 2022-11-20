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
            SelectedLineItem = items.Count > 0 ? items[0] : NullLineItem.Instance;
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
        private int InternalSelectedIndex { get; set; }
        public int SelectedIndex { get { return InternalSelectedIndex; } set { SelectIndex(value); } }
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
        public static NullLineItem Instance { get; } = new NullLineItem();
        public string Text { get => string.Empty; init { } }
        public bool IsApproved { get => false; set { } }
        public bool IsTranslated { get => false; set { } }
        public bool IsSearchResult { get => false; set { } }
        public EventHandler OnClick { get { return new((object? sender, EventArgs e) => { }); } init { } }

        public void Approve() { }
        public void Unapprove() { }
    }

    public class NullTabController : ITabController
    {
        public static NullTabController Instance { get; } = new NullTabController();

        public ITab SelectedTab { get => NullTab.Instance; set { } }

        List<ITab> ITabController.TabPages { get; } = new();

        public bool CloseTab(ITab tab) => throw new NotImplementedException();
    }

    public interface ITabController
    {
        ITab SelectedTab { get; set; }
        List<ITab> TabPages { get; }
        int SelectedIndex { get; set; }
        int TabCount { get; }

        bool CloseTab(ITab tab);
    }

    public class NullTab : ITab
    {
        public static NullTab Instance { get; } = new NullTab();

        public void Dispose() => throw new NotImplementedException();
    }
    public interface ITab
    {
        string Text { get; set; }

        void Dispose();
    }

    public class NullUIHandler : IUIHandler
    {
        public static NullUIHandler Instance { get; } = new NullUIHandler();

        public void ApproveSelectedLine() => throw new NotImplementedException();
        public void ClearLines() => throw new NotImplementedException();
        public PopupResult ErrorOk(string message, string title = "Error") => throw new NotImplementedException();
        public PopupResult ErrorOkCancel(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult ErrorYesNo(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult ErrorYesNoCancel(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public void FocusCommentBox() => throw new NotImplementedException();
        public void FocusReplaceBar() => throw new NotImplementedException();
        public void FocusSearchBar() => throw new NotImplementedException();
        public void FocusTranslationBox() => throw new NotImplementedException();
        public string GetCommentBoxText() => throw new NotImplementedException();
        public MenuItems GetFileMenuItems() => throw new NotImplementedException();
        public ILineItem GetLineItem(int index) => throw new NotImplementedException();
        public LineList GetLines() => throw new NotImplementedException();
        public string GetReplaceBarText() => throw new NotImplementedException();
        public string GetSearchBarText() => throw new NotImplementedException();
        public string GetTemplateBoxText() => throw new NotImplementedException();
        public string GetTranslationBoxText() => throw new NotImplementedException();
        public PopupResult InfoOk(string message, string title = "Info") => throw new NotImplementedException();
        public PopupResult InfoOkCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult InfoYesNo(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public string SelectedCommentBoxText() => throw new NotImplementedException();
        public int SelectedLineIndex() => throw new NotImplementedException();
        public ILineItem SelectedLineItem() => throw new NotImplementedException();
        public string SelectedTemplateBoxText() => throw new NotImplementedException();
        public string SelectedTranslationBoxText() => throw new NotImplementedException();
        public void SelectLineItem(int index) => throw new NotImplementedException();
        public void SelectLineItem(ILineItem item) => throw new NotImplementedException();
        public void SetCommentBoxText(string text) => throw new NotImplementedException();
        public void SetFileMenuItems(MenuItems menuItems) => throw new NotImplementedException();
        public void SetLines(LineList lines) => throw new NotImplementedException();
        public void SetReplaceBarText(string replacement) => throw new NotImplementedException();
        public void SetSearchBarText(string query) => throw new NotImplementedException();
        public void SetSelectedCommentBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTemplateBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTranslationBoxText(int start, int end) => throw new NotImplementedException();
        public void SetTemplateBoxText(string text) => throw new NotImplementedException();
        public void SetTitle(string title) => throw new NotImplementedException();
        public void SetTranslationBoxText(string text) => throw new NotImplementedException();
        public void SignalAppExit() => throw new NotImplementedException();
        public void SignalUserEndWait() => throw new NotImplementedException();
        public void SignalUserWait() => throw new NotImplementedException();
        public void UnapproveSelectedLine() => throw new NotImplementedException();
        public void Update() => throw new NotImplementedException();
        public void UpdateLines() => throw new NotImplementedException();
        public void UpdateResults() => throw new NotImplementedException();
        public PopupResult WarningOk(string message, string title = "Warning") => throw new NotImplementedException();
        public PopupResult WarningOkCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult WarningYesNo(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
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
        void ClipboardSetText(string text);
        #endregion

        #region tabs

        #endregion
    }
}
