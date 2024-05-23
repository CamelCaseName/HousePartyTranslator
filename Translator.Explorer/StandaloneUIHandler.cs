using System.Media;
using Translator.Core;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Explorer
{
    internal class StandaloneUIHandler : IUIHandler
    {
        public MenuItems FileMenuItems => throw new NotImplementedException();

        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ReplaceBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SearchBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SearchResultCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SelectedSearchResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Type? publicFileDialogType => throw new NotImplementedException();

        public Type? publicFolderDialogType => throw new NotImplementedException();

        public Type? publicSaveFileDialogType => throw new NotImplementedException();

        public CreateTemplateFromStoryDelegate CreateTemplateFromStory => throw new NotImplementedException();

        public ITabController TabControl => throw new NotImplementedException();

        public string Language { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ReplaceBarIsVisible => throw new NotImplementedException();

        public int TranslationBoxTextLength => throw new NotImplementedException();

        public int TemplateBoxTextLength => throw new NotImplementedException();

        public int TemplateBoxSelectedTextLength => throw new NotImplementedException();

        public int TranslationBoxSelectedTextLength => throw new NotImplementedException();

        public ITab SelectedTab => throw new NotImplementedException();

        public string TranslationBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TemplateBoxText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
        public PopupResult InfoOk(string message, string title = "Info") => throw new NotImplementedException();
        public PopupResult InfoOkCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult InfoYesNo(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public void SetLanguageHighlights(string[] languages) => throw new NotImplementedException();
        public void SetReplaceMenuInVisible() => throw new NotImplementedException();
        public void SetReplaceMenuVisible() => throw new NotImplementedException();
        public void SetSelectedSearchBarText(int v, int length) => throw new NotImplementedException();
        public void SignalAppExit() => Application.Exit();
        public void SignalUserEndWait() => Application.UseWaitCursor = true;
        public void SignalUserPing() => SystemSounds.Exclamation.Play();
        public void SignalUserWait() => Application.UseWaitCursor = false;
        public void ToggleReplaceBar() => throw new NotImplementedException();
        public void Update() => throw new NotImplementedException();
        public void UpdateRecentFileList() => throw new NotImplementedException();
        public PopupResult WarningOk(string message, string title = "Warning") => throw new NotImplementedException();
        public PopupResult WarningOkCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult WarningYesNo(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
    }
}
