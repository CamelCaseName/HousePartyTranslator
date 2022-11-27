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
        public string FileName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string InitialDirectory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Extension { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool PromptCreate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool PromptOverwrite { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PopupResult ShowDialog() => throw new NotImplementedException();
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
        public EventHandler OnClick { get => new((object? sender, EventArgs e) => { }); init { } }
        public string Text { get => string.Empty; set { } }
    }

    public class NullTab : ITab<NullLineItem>
    {
        public static NullTab Instance { get; } = new NullTab();
        public string Text { get => ""; set { } }

        public bool IsApproveButtonFocused => throw new NotImplementedException();

        public List<string> SimilarStringsToEnglish => throw new NotImplementedException();

        public int SelectedLineIndex => throw new NotImplementedException();

        public NullLineItem SelectedLineItem => throw new NotImplementedException();

        public bool IsTranslationBoxFocused => throw new NotImplementedException();

        public bool IsCommentBoxFocused => throw new NotImplementedException();

        public int ProgressValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ApproveSelectedLine() => throw new NotImplementedException();
        public void ClearLines() => throw new NotImplementedException();

        public void Dispose() { }

        public void FocusCommentBox() => throw new NotImplementedException();
        public void FocusTranslationBox() => throw new NotImplementedException();
        public string CommentBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ILineItem GetLineItem(int index) => throw new NotImplementedException();
        public LineList<NullLineItem> Lines { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TemplateBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TranslationBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string SelectedCommentBoxText() => throw new NotImplementedException();
        public int GetSelectedLineIndex() => throw new NotImplementedException();
        public ILineItem GetSelectedLineItem() => throw new NotImplementedException();
        public string SelectedTemplateBoxText => throw new NotImplementedException();
        public string SelectedTranslationBoxText => throw new NotImplementedException();
        public void SelectLineItem(int index) => throw new NotImplementedException();
        public void SelectLineItem(ILineItem item) => throw new NotImplementedException();
        public void SetSelectedCommentBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTemplateBoxText(int start, int end) => throw new NotImplementedException();
        public void SetSelectedTranslationBoxText(int start, int end) => throw new NotImplementedException();
        public void UnapproveSelectedLine() => throw new NotImplementedException();
        public void UpdateLines() => throw new NotImplementedException();
        NullLineItem ITab<NullLineItem>.AtIndex(int index) => throw new NotImplementedException();
        public void SelectLineItem(NullLineItem item) => throw new NotImplementedException();
        public string[] CommentBoxTextArr => throw new NotImplementedException();
        public void SetCommentBoxText(string[] lines) => throw new NotImplementedException();
        public bool ApprovedButtonChecked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string[] ITab<NullLineItem>.CommentBoxTextArr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SetFileInfoText(string info) => throw new NotImplementedException();
        public void SetApprovedLabelText(string v) => throw new NotImplementedException();
        public void SetCharacterLabelColor(Color lawnGreen) => throw new NotImplementedException();
        public void SetCharacterCountLabelText(string v) => throw new NotImplementedException();
    }

    public class NullTabController : ITabController<NullLineItem>
    {
        public static NullTabController Instance { get; } = new NullTabController();

        public int SelectedIndex { get => 0; set { } }
        public ITab<NullLineItem> SelectedTab { get => (ITab<NullLineItem>)NullTab.Instance; set { } }
        public int TabCount => 0;

        public List<ITab<NullLineItem>> TabPages => throw new NotImplementedException();

        List<ITab<NullLineItem>> ITabController<NullLineItem>.TabPages { get; } = new();

        public bool CloseTab(ITab<NullLineItem> tab) { return false; }
    }

#pragma warning disable IDE0060
    public class NullUIHandler : IUIHandler<NullLineItem>
    {
        public static NullUIHandler Instance { get; } = new NullUIHandler();
        public Type FileDialogType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public Type FolderDialogType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public ITabController<NullLineItem> TabControl => throw new NotImplementedException();

        public string Language { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CreateTemplateDataDelegate CreateTemplateData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ReplaceBarIsVisible => throw new NotImplementedException();

        ITabController<NullLineItem> IUIHandler<NullLineItem>.TabControl => throw new NotImplementedException();

        Type IUIHandler<NullLineItem>.InternalFileDialogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Type IUIHandler<NullLineItem>.FileDialogType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        Type IUIHandler<NullLineItem>.InternalFolderDialogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Type IUIHandler<NullLineItem>.FolderDialogType { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        Type IUIHandler<NullLineItem>.InternalSaveFileDialogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IUIHandler<NullLineItem>.Language { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ClipboardSetText(string text) => throw new NotImplementedException();
        public ITab<NullLineItem>? CreateNewTab() => throw new NotImplementedException();
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
        public void SetReplaceMenuInVisible() => throw new NotImplementedException();
        public void SetReplaceMenuVisible() => throw new NotImplementedException();
        public void SetSearchBarText(string query) => throw new NotImplementedException();
        public void SetTitle(string title) => throw new NotImplementedException();
        public void SignalAppExit() => throw new NotImplementedException();
        public void SignalUserEndWait() => throw new NotImplementedException();
        public void SignalUserWait() => throw new NotImplementedException();
        public void Update() => throw new NotImplementedException();
        public void UpdateProgress() => throw new NotImplementedException();
        public void UpdateResults() => throw new NotImplementedException();
        public PopupResult WarningOk(string message, string title = "Warning") => throw new NotImplementedException();
        public PopupResult WarningOkCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult WarningYesNo(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.ClipboardSetText(string text) => throw new NotImplementedException();
        ITab<NullLineItem>? IUIHandler<NullLineItem>.CreateNewTab() => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.ErrorOk(string message, string title) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.ErrorOkCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.ErrorOkCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.ErrorYesNo(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.ErrorYesNo(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.ErrorYesNoCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.ErrorYesNoCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.FocusReplaceBar() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.FocusSearchBar() => throw new NotImplementedException();
        MenuItems IUIHandler<NullLineItem>.FileMenuItems { get => throw new NotImplementedException(); set { } }
        string IUIHandler<NullLineItem>.ReplaceBarText { get => throw new NotImplementedException(); set { } }
        string IUIHandler<NullLineItem>.SearchBarText { get => throw new NotImplementedException(); set { } }

        public MenuItems FileMenuItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ReplaceBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SearchBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        PopupResult IUIHandler<NullLineItem>.InfoOk(string message, string title) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.InfoOkCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.InfoOkCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.InfoYesNo(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.InfoYesNo(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.InfoYesNoCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.InfoYesNoCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.Login() => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.Logout() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.SetTitle(string title) => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.SignalAppExit() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.SignalUserEndWait() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.SignalUserWait() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.Update() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.UpdateProgress() => throw new NotImplementedException();
        void IUIHandler<NullLineItem>.UpdateResults() => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.WarningOk(string message, string title) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.WarningOkCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.WarningOkCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.WarningYesNo(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.WarningYesNo(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<NullLineItem>.WarningYesNoCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<NullLineItem>.WarningYesNoCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
    }
#pragma warning restore
}