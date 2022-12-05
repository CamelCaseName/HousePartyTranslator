using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Translator;
using Translator.Core;
using Translator.Core.Helpers;
using Translator.Helpers;
using Translator.Managers;
using Translator.UICompatibilityLayer;
using TranslatorAdmin.Managers;

namespace TranslatorAdmin.InterfaceImpls
{

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal sealed class WinUIHandler : IUIHandler<WinLineItem>
    {
        #region interface
        public MenuItems FileMenuItems => (MenuItems)(App.MainForm.MainMenuStrip?.Items.Cast<IMenuItem>() ?? new MenuItems());

        public string ReplaceBarText { get => App.MainForm.ReplaceBox.Text; set => App.MainForm.ReplaceBox.Text = value; }
        public string SearchBarText { get => App.MainForm.SearchBox.Text; set => App.MainForm.SearchBox.Text = value; }

        private WinTranslationManager WinTranslation => WinTranslation ?? new(this);
        public CreateTemplateFromStoryDelegate CreateTemplateFromStory { get => WinTranslation.CreateTemplateFromStory; }

        public ITabController<WinLineItem> TabControl => App.MainForm.TabControl;

        public string Language { get => App.MainForm.LanguageBox.Text; set => App.MainForm.LanguageBox.Text = value; }

        public bool ReplaceBarIsVisible => App.MainForm.ReplaceAllButton.Visible && App.MainForm.ReplaceButton.Visible && App.MainForm.ReplaceBox.Visible;

        Type IUIHandler<WinLineItem>.InternalFileDialogType => typeof(WinFileDialog);
        Type IUIHandler<WinLineItem>.InternalFolderDialogType => typeof(WinFolderDialog);
        Type IUIHandler<WinLineItem>.InternalSaveFileDialogType => typeof(WinSaveFileDialog);

        public void ClipboardSetText(string text) => Clipboard.SetText(text);
        public ITab<WinLineItem>? CreateNewTab() => new WinTab();
        public PopupResult ErrorOk(string message, string title = "Error") => Msg.ErrorOk(message, title).ToPopupResult();
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
        public void SetReplaceMenuInVisible() => throw new NotImplementedException();
        public void SetReplaceMenuVisible() => throw new NotImplementedException();
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
        #endregion



    }
}
