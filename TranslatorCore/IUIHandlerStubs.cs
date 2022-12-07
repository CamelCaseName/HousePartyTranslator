using System.Collections.Generic;
using System;
using Translator.Core;
using System.Drawing;

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

    public class NullSaveFileDialog : ISaveFileDialog
    {
        public string FileName { get { return string.Empty; } set { } }
        public string InitialDirectory { get { return string.Empty; } set { } }
        public string Title { get { return string.Empty; } set { } }
        public string Extension { get { return string.Empty; } set { } }
        public bool PromptCreate { get { return false; } set { } }
        public bool PromptOverwrite { get { return false; } set { } }

        public PopupResult ShowDialog() { return PopupResult.NONE; }
    }

    public class NullLineItem : ILineItem
    {
        public static NullLineItem Instance { get; } = new NullLineItem();
        public bool IsApproved { get => false; set { } }
        public bool IsSearchResult { get => false; set { } }
        public bool IsTranslated { get => false; set { } }
        public string Text { get => string.Empty; init { } }
        public void Approve() { }
        public void Unapprove() { }
    }

    public class NullMenuItem : IMenuItem
    {
        public static NullMenuItem Instance { get; } = new();
        public event EventHandler Click { add { } remove { } }
        public string Text { get => string.Empty; set { } }
    }

    public class NullTab : ITab<NullLineItem>
    {
        public static NullTab Instance { get; } = new NullTab();
        public string Text { get => ""; set { } }

        public bool IsApproveButtonFocused => false;

        public List<string> SimilarStringsToEnglish => new();

        public int SelectedLineIndex => 0;

        public NullLineItem SelectedLineItem => NullLineItem.Instance;

        public bool IsTranslationBoxFocused => false;

        public bool IsCommentBoxFocused => false;

        public int ProgressValue { get { return 0; } set { } }

        public void ApproveSelectedLine() { }
        public void ClearLines() { }

        public void Dispose() { }

        public void FocusCommentBox() { }
        public void FocusTranslationBox() { }
        public string CommentBoxText { get { return string.Empty; } set { } }

        public ILineList<NullLineItem> Lines { get { return new NullLineList<NullLineItem>(); } set { } }
        public string TemplateBoxText { get { return string.Empty; } set { } }
        public string TranslationBoxText { get { return string.Empty; } set { } }

        public string SelectedCommentBoxText() { return string.Empty; }
        public void SelectLineItem(int index) { }
        public void SetSelectedCommentBoxText(int start, int end) { }
        public void SetSelectedTemplateBoxText(int start, int end) { }
        public void SetSelectedTranslationBoxText(int start, int end) { }
        public void UnapproveSelectedLine() { }
        public void UpdateLines() { }
        NullLineItem ITab<NullLineItem>.AtIndex(int index) { return NullLineItem.Instance; }
        public void SelectLineItem(NullLineItem item) { }
        public bool ApprovedButtonChecked { get { return false; } set { } }
        string[] ITab<NullLineItem>.CommentBoxTextArr { get { return Array.Empty<string>(); } set { } }

        string ITab<NullLineItem>.SelectedTranslationBoxText => string.Empty;

        string ITab<NullLineItem>.SelectedTemplateBoxText => string.Empty;

        public void SetFileInfoText(string info) { }
        public void SetApprovedLabelText(string v) { }
        public void SetCharacterLabelColor(Color lawnGreen) { }
        public void SetCharacterCountLabelText(string v) { }
    }

    public class NullTabController : ITabController<NullLineItem>
    {
        public static NullTabController Instance { get; } = new NullTabController();

        public int SelectedIndex { get => 0; set { } }
        public ITab<NullLineItem> SelectedTab { get => (ITab<NullLineItem>)NullTab.Instance; set { } }
        public int TabCount => 0;

        List<ITab<NullLineItem>> ITabController<NullLineItem>.TabPages { get; } = new();

        public bool CloseTab(ITab<NullLineItem> tab) { return false; }
    }

    public class NullUIHandler : IUIHandler<NullLineItem>
    {
        public static NullUIHandler Instance { get; } = new NullUIHandler();

        public string ReplaceBarText { get { return string.Empty; } set { } }
        public string SearchBarText { get { return string.Empty; } set { } }

        public Type? InternalFileDialogType => null;

        public Type? InternalFolderDialogType => null;

        public Type? InternalSaveFileDialogType => null;

        CreateTemplateFromStoryDelegate IUIHandler<NullLineItem>.CreateTemplateFromStory => throw new NotImplementedException();

        ITabController<NullLineItem> IUIHandler<NullLineItem>.TabControl => NullTabController.Instance;

        public string Language { get { return string.Empty; } set { } }

        bool IUIHandler<NullLineItem>.ReplaceBarIsVisible => false;

        int IUIHandler<NullLineItem>.TranslationBoxTextLength => 0;

        int IUIHandler<NullLineItem>.TemplateBoxTextLength => 0;

        int IUIHandler<NullLineItem>.TemplateBoxSelectedTextLength => 0;

        int IUIHandler<NullLineItem>.TranslationBoxSelectedTextLength => 0;

        ITab<NullLineItem> IUIHandler<NullLineItem>.SelectedTab => NullTab.Instance;

        MenuItems IUIHandler<NullLineItem>.FileMenuItems => throw new NotImplementedException();

        public string TranslationBoxText { get => string.Empty; set { } }
        public string TemplateBoxText { get => string.Empty; set { } }

        public void ClipboardSetText(string text) { }
        public ITab<NullLineItem>? CreateNewTab() { return NullTab.Instance; }
        public PopupResult ErrorOk(string message, string title = "Error") { return PopupResult.NONE; }
        public PopupResult ErrorOkCancel(string message, string title = "Error") { return PopupResult.NONE; }
        public bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK) { return false; }
        public PopupResult ErrorYesNo(string message, string title = "Error") { return PopupResult.NONE; }
        public bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES) { return false; }
        public PopupResult ErrorYesNoCancel(string message, string title = "Error") { return PopupResult.NONE; }
        public bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES) { return false; }
        public void FocusReplaceBar() { }
        public void FocusSearchBar() { }
        public PopupResult InfoOk(string message, string title = "Info") { return PopupResult.NONE; }
        public PopupResult InfoOkCancel(string message, string title = "Info") { return PopupResult.NONE; }
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) { return false; }
        public PopupResult InfoYesNo(string message, string title = "Info") { return PopupResult.NONE; }
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) { return false; }
        public PopupResult InfoYesNoCancel(string message, string title = "Info") { return PopupResult.NONE; }
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) { return false; }
        public bool Login() { return false; }
        public bool Logout() { return false; }
        public void SetReplaceMenuInVisible() { }
        public void SetReplaceMenuVisible() { }
        public void SetTitle(string title) { }
        public void SignalAppExit() { }
        public void SignalUserEndWait() { }
        public void SignalUserWait() { }
        public void Update() { }
        public void UpdateTranslationProgressIndicator() { }
        public void UpdateResults() { }
        public PopupResult WarningOk(string message, string title = "Warning") { return PopupResult.NONE; }
        public PopupResult WarningOkCancel(string message, string title = "Warning") { return PopupResult.NONE; }
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) { return false; }
        public PopupResult WarningYesNo(string message, string title = "Warning") { return PopupResult.NONE; }
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) { return false; }
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") { return PopupResult.NONE; }
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) { return false; }
    }
}