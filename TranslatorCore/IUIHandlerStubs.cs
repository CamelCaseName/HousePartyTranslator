using System.Collections.Generic;
using System;

namespace Translator.UICompatibilityLayer.StubImpls
{
    public class NullFileDialog : IFileDialog
    {
        public static NullFileDialog Instance { get; } = new();
        public string FileName { get => string.Empty; set { } }
        public string Filter { get => string.Empty; set { } }
        public string InitialDirectory { get => string.Empty; set { } }
        public string SelectedPath => string.Empty;
        public string Title { get => string.Empty; set { } }
        public PopupResult ShowDialog() => PopupResult.NONE;
    }

    public class NullFolderDialog : IFolderDialog
    {
        public static NullFolderDialog Instance { get; } = new();
        public string SelectedFolderPath { get => string.Empty; set { } }
        public string Text { get => string.Empty; set { } }
        public PopupResult ShowDialog() => PopupResult.NONE;
    }

    public class NullLineItem : ILineItem
    {
        public static NullLineItem Instance { get; } = new NullLineItem();
        public bool IsApproved { get => false; set { } }
        public bool IsSearchResult { get => false; set { } }
        public bool IsTranslated { get => false; set { } }
        public EventHandler OnClick { get { return new((sender, e) => { }); } init { } }
        public string Text { get => string.Empty; init { } }
        public void Approve() { }
        public void Unapprove() { }
    }

    public class NullMenuItem : IMenuItem
    {
        public static NullMenuItem Instance { get; } = new();
        public EventHandler OnClick { get => new((object? sender, EventArgs e)=> { }); init { } }
        public string Text { get => string.Empty; set { } }
    }

    public class NullTab : ITab
    {
        public static NullTab Instance { get; } = new NullTab();
        public string Text { get => ""; set { } }

        public void ApproveSelectedLine() => throw new NotImplementedException();
        public void ClearLines() => throw new NotImplementedException();

        public void Dispose() { }

        public void FocusCommentBox() => throw new NotImplementedException();
        public void FocusTranslationBox() => throw new NotImplementedException();
        public string GetCommentBoxText() => throw new NotImplementedException();
        public ILineItem GetLineItem(int index) => throw new NotImplementedException();
        public LineList GetLines() => throw new NotImplementedException();
        public string GetTemplateBoxText() => throw new NotImplementedException();
        public string GetTranslationBoxText() => throw new NotImplementedException();
        public string SelectedCommentBoxText() => throw new NotImplementedException();
        public int SelectedLineIndex() => throw new NotImplementedException();
        public ILineItem SelectedLineItem() => throw new NotImplementedException();
        public string SelectedTemplateBoxText() => throw new NotImplementedException();
        public string SelectedTranslationBoxText() => throw new NotImplementedException();
        public void SelectLineItem(int index) => throw new NotImplementedException();
        public void SelectLineItem(ILineItem item) => throw new NotImplementedException();
        public void SetCommentBoxText(string text) => throw new NotImplementedException();
        public void SetLines(LineList lines) => throw new NotImplementedException();
        public void SetSelectedCommentBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTemplateBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTranslationBoxText(int start, int end) => throw new NotImplementedException();
        public void SetTemplateBoxText(string text) => throw new NotImplementedException();
        public void SetTranslationBoxText(string text) => throw new NotImplementedException();
        public void UnapproveSelectedLine() => throw new NotImplementedException();
        public void UpdateLines() => throw new NotImplementedException();
    }

    public class NullTabController : ITabController
    {
        public static NullTabController Instance { get; } = new NullTabController();

        public int SelectedIndex { get => 0; set { } }
        public ITab SelectedTab { get => NullTab.Instance; set { } }
        public int TabCount => 0;

        List<ITab> ITabController.TabPages { get; } = new();

        public bool CloseTab(ITab tab) { return false; }
    }

    public class NullUIHandler : IUIHandler
    {
        public static NullUIHandler Instance { get; } = new NullUIHandler();
        public Type FileDialogType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public Type FolderDialogType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public ITabController TabControl => throw new NotImplementedException();

        public void ClipboardSetText(string text) => throw new NotImplementedException();
        public ITab? CreateNewTab() => throw new NotImplementedException();
        public PopupResult ErrorOk(string message, string title = "Error") => throw new NotImplementedException();
        public PopupResult ErrorOkCancel(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult ErrorYesNo(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult ErrorYesNoCancel(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public void FocusReplaceBar() => throw new NotImplementedException();
        public void FocusSearchBar() => throw new NotImplementedException();
        public MenuItems GetFileMenuItems() => throw new NotImplementedException();
        public string GetReplaceBarText() => throw new NotImplementedException();
        public string GetSearchBarText() => throw new NotImplementedException();
        public PopupResult InfoOk(string message, string title = "Info") => throw new NotImplementedException();
        public PopupResult InfoOkCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult InfoYesNo(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public void SetFileMenuItems(MenuItems menuItems) => throw new NotImplementedException();
        public void SetReplaceBarText(string replacement) => throw new NotImplementedException();
        public void SetSearchBarText(string query) => throw new NotImplementedException();
        public void SetTitle(string title) => throw new NotImplementedException();
        public void SignalAppExit() => throw new NotImplementedException();
        public void SignalUserEndWait() => throw new NotImplementedException();
        public void SignalUserWait() => throw new NotImplementedException();
        public void Update() => throw new NotImplementedException();
        public void UpdateResults() => throw new NotImplementedException();
        public PopupResult WarningOk(string message, string title = "Warning") => throw new NotImplementedException();
        public PopupResult WarningOkCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult WarningYesNo(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
    }
}