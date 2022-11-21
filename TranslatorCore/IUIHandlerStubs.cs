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

    public class NullTab : ITab
    {
        public static NullTab Instance { get; } = new NullTab();
        public string Text { get => ""; set { } }

        public void Dispose() { }
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

        public Type FileDialogType { get => typeof(NullFileDialog); init { } }
        public Type FolderDialogType { get => typeof(NullFolderDialog); init { } }
        public ITabController TabControl => throw new NotImplementedException();
        public void ApproveSelectedLine() => throw new NotImplementedException();
        public void ClearLines() => throw new NotImplementedException();
        public void ClipboardSetText(string text) => throw new NotImplementedException();
        public ITab? CreateNewTab() => throw new NotImplementedException();
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
        public string GetCommentBoxText() => string.Empty;
        public MenuItems GetFileMenuItems() => throw new NotImplementedException();
        public ILineItem GetLineItem(int index) => throw new NotImplementedException();
        public LineList GetLines() => throw new NotImplementedException();
        public string GetReplaceBarText() => string.Empty;
        public string GetSearchBarText() => string.Empty;
        public string GetTemplateBoxText() => string.Empty;
        public string GetTranslationBoxText() => string.Empty;
        public PopupResult InfoOk(string message, string title = "Info") => throw new NotImplementedException();
        public PopupResult InfoOkCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult InfoYesNo(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public string SelectedCommentBoxText() => string.Empty;
        public int SelectedLineIndex() => throw new NotImplementedException();
        public ILineItem SelectedLineItem() => throw new NotImplementedException();
        public string SelectedTemplateBoxText() => string.Empty;
        public string SelectedTranslationBoxText() => string.Empty;
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
}