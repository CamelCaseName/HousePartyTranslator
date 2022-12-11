using Translator;
using Translator.Core;
using Translator.Helpers;
using Translator.UICompatibilityLayer;
using TranslatorAdmin.Managers;

namespace TranslatorAdmin.InterfaceImpls
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal sealed class WinUIHandler : IUIHandler<WinLineItem, WinTabController, WinTab>
    {
        public WinUIHandler() { }
        
        internal WinUIHandler(ITabController<WinLineItem, WinTab> control)
        {
            TabControl = (WinTabController)control;
        }

        #region interface
        //todo replace update of the menuitems by a change, then save new ones, not using reference
        public MenuItems FileMenuItems => (MenuItems)(App.MainForm.MainMenuStrip?.Items.Cast<IMenuItem>() ?? new MenuItems());

        public string ReplaceBarText { get => App.MainForm.ReplaceBox.Text; set => App.MainForm.ReplaceBox.Text = value; }

        public string SearchBarText { get => App.MainForm.SearchBox.Text; set => App.MainForm.SearchBox.Text = value; }

        private WinTranslationManager WinTranslation => WinTranslation ?? new(this);

        public CreateTemplateFromStoryDelegate CreateTemplateFromStory { get => WinTranslation.CreateTemplateFromStory; }

        public WinTabController TabControl { get; } = new();

        public string Language { get => App.MainForm.LanguageBox.Text; set => App.MainForm.LanguageBox.Text = value; }

        public bool ReplaceBarIsVisible => App.MainForm.ReplaceAllButton.Visible && App.MainForm.ReplaceButton.Visible && App.MainForm.ReplaceBox.Visible;

        Type IUIHandler<WinLineItem, WinTabController, WinTab>.InternalFileDialogType => typeof(WinFileDialog);
        Type IUIHandler<WinLineItem, WinTabController, WinTab>.InternalFolderDialogType => typeof(WinFolderDialog);
        Type IUIHandler<WinLineItem, WinTabController, WinTab>.InternalSaveFileDialogType => typeof(WinSaveFileDialog);

        public int TranslationBoxTextLength => TabControl.SelectedTab.TranslationBoxText.Length;

        public int TemplateBoxTextLength => TabControl.SelectedTab.TemplateBoxText.Length;

        public int TemplateBoxSelectedTextLength => TabControl.SelectedTab.SelectedTranslationBoxText.Length;

        public int TranslationBoxSelectedTextLength => TabControl.SelectedTab.SelectedTemplateBoxText.Length;

        public WinTab SelectedTab => TabControl.SelectedTab;

        public string TranslationBoxText { get => TabControl.SelectedTab.TranslationBoxText; set => TabControl.SelectedTab.TranslationBoxText = value; }
        public string TemplateBoxText { get => TabControl.SelectedTab.TranslationBoxText; set => TabControl.SelectedTab.TranslationBoxText = value; }

        public void ClipboardSetText(string text) => Clipboard.SetText(text);
        public WinTab? CreateNewTab() => new(App.MainForm);
        public PopupResult ErrorOk(string message, string title = "Error") => Msg.ErrorOk(message, title).ToPopupResult();
        public PopupResult ErrorOkCancel(string message, string title = "Error") => Msg.ErrorOkCancel(message, title).ToPopupResult();
        public bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK) => Msg.ErrorOkCancelB(message, title, result.ToDialogResult());
        public PopupResult ErrorYesNo(string message, string title = "Error") => Msg.ErrorYesNo(message, title).ToPopupResult();
        public bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES) => Msg.ErrorYesNoB(message, title, result.ToDialogResult());
        public PopupResult ErrorYesNoCancel(string message, string title = "Error") => Msg.ErrorYesNoCancel(message, title).ToPopupResult();
        public bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES) => Msg.ErrorYesNoCancelB(message, title, result.ToDialogResult());
        public void FocusReplaceBar() => App.MainForm.ReplaceBox.Focus();
        public void FocusSearchBar() => App.MainForm.SearchBox.Focus();
        public PopupResult InfoOk(string message, string title = "Info") => Msg.InfoOk(message, title).ToPopupResult();
        public PopupResult InfoOkCancel(string message, string title = "Info") => Msg.InfoOkCancel(message, title).ToPopupResult();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => Msg.InfoOkCancelB(message, title, result.ToDialogResult());
        public PopupResult InfoYesNo(string message, string title = "Info") => Msg.InfoYesNo(message, title).ToPopupResult();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => Msg.InfoYesNoB(message, title, result.ToDialogResult());
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => Msg.InfoYesNoCancel(message, title).ToPopupResult();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => Msg.InfoYesNoCancelB(message, title, result.ToDialogResult());
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public void SetReplaceMenuInVisible() => App.MainForm.ReplaceAllButton.Visible = App.MainForm.ReplaceButton.Visible = App.MainForm.ReplaceBox.Visible = false;
        public void SetReplaceMenuVisible() => App.MainForm.ReplaceAllButton.Visible = App.MainForm.ReplaceButton.Visible = App.MainForm.ReplaceBox.Visible = true;
        public void SetTitle(string title) => App.MainForm.Text = title;
        public void SignalAppExit() => Application.Exit();
        public void SignalUserEndWait() => Application.UseWaitCursor = false;
        public void SignalUserWait() => Application.UseWaitCursor = true;
        public void Update() => App.MainForm.Update();
        public void UpdateTranslationProgressIndicator() => SelectedTab.ProgressbarTranslated.Update();
        public void UpdateResults() => SelectedTab.Lines.Update();

        public PopupResult WarningOk(string message, string title = "Warning") => Msg.WarningOk(message, title).ToPopupResult();
        public PopupResult WarningOkCancel(string message, string title = "Warning") => Msg.WarningOkCancel(message, title).ToPopupResult();
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => Msg.WarningOkCancelB(message, title, result.ToDialogResult());
        public PopupResult WarningYesNo(string message, string title = "Warning") => Msg.WarningYesNo(message, title).ToPopupResult();
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => Msg.WarningYesNoB(message, title, result.ToDialogResult());
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => Msg.WarningYesNoCancel(message, title).ToPopupResult();
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => Msg.WarningYesNoCancelB(message, title, result.ToDialogResult());
        public void SetFileMenuItems(MenuItems menuItems) {
            App.MainForm.FileToolStripMenuItem.DropDownItems.Clear();
            App.MainForm.FileToolStripMenuItem.DropDownItems.AddRange(menuItems.ToToolStripItemCollection(App.MainForm.MainMenu ?? new MenuStrip()));
        }
        #endregion



    }
}
