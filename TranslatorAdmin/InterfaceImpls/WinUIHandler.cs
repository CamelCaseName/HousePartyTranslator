using System;
using System.Windows.Forms;
using Translator.Core;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.Managers;
using Translator.Desktop.UI.Components;
using Translator.Helpers;

namespace Translator.Desktop.InterfaceImpls
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public sealed class WinUIHandler : IUIHandler
    {
        private int waitCounter = 0;
        public WinUIHandler() { }

        internal WinUIHandler(ITabController control)
        {
            TabControl = (WinTabController)control;
        }

        public CreateTemplateFromStoryDelegate CreateTemplateFromStory { get => WinTranslationManager.CreateTemplateFromStory; }
        public MenuItems FileMenuItems => App.MainForm?.FileToolStripMenuItem.DropDownItems.ToMenuItems() ?? new MenuItems();

        Type IUIHandler.InternalFileDialogType => typeof(WinFileDialog);
        Type IUIHandler.InternalFolderDialogType => typeof(WinFolderDialog);
        Type IUIHandler.InternalSaveFileDialogType => typeof(WinSaveFileDialog);
        public string Language { get => App.MainForm?.LanguageBox.Text ?? string.Empty; set { _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the language"); App.MainForm.LanguageBox.Text = value; } }
        public bool ReplaceBarIsVisible => App.MainForm?.ReplaceAllButton.Visible ?? false && App.MainForm.ReplaceButton.Visible && App.MainForm.ReplaceBox.Visible;
        public string ReplaceBarText { get => App.MainForm?.ReplaceBox.Text ?? string.Empty; set { _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the ReplaceBarText"); App.MainForm.ReplaceBox.Text = value; } }

        public string SearchBarText { get => App.MainForm?.SearchBox.Text ?? string.Empty; set { _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the SearchBArText"); App.MainForm.SearchBox.Text = value; } }
        public int SearchResultCount { get => App.MainForm?.SearchBox.TotalSearchResults ?? 0; set { App.MainForm.SearchBox.TotalSearchResults = value; App.MainForm?.SearchBox.Invalidate(); } }
        public int SelectedSearchResult { get => App.MainForm?.SearchBox.CurrentSearchResult ?? 0; set { App.MainForm.SearchBox.CurrentSearchResult = value; App.MainForm?.SearchBox.Invalidate(); } }
        public ITab SelectedTab => TabControl.SelectedTab;
        public ITabController TabControl { get; } = new WinTabController();
        public int TemplateBoxSelectedTextLength => TabControl.SelectedTab.SelectedTranslationBoxText.Length;
        public string TemplateBoxText { get => TabControl.SelectedTab.TranslationBoxText; set => TabControl.SelectedTab.TranslationBoxText = value; }
        public int TemplateBoxTextLength => TabControl.SelectedTab.TemplateBoxText.Length;
        public int TranslationBoxSelectedTextLength => TabControl.SelectedTab.SelectedTemplateBoxText.Length;
        public string TranslationBoxText { get => TabControl.SelectedTab.TranslationBoxText; set => TabControl.SelectedTab.TranslationBoxText = value; }
        public int TranslationBoxTextLength => TabControl.SelectedTab.TranslationBoxText.Length;
        public void ClipboardSetText(string text) => Clipboard.SetText(text);
        public ITab? CreateNewTab()
        {
            if (App.MainForm == null) return null;
            return new WinTab(App.MainForm);
        }

        public PopupResult ErrorOk(string message, string title = "Error") => Msg.ErrorOk(message, title).ToPopupResult();
        public PopupResult ErrorOkCancel(string message, string title = "Error") => Msg.ErrorOkCancel(message, title).ToPopupResult();
        public bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK) => Msg.ErrorOkCancelB(message, title, result.ToDialogResult());
        public PopupResult ErrorYesNo(string message, string title = "Error") => Msg.ErrorYesNo(message, title).ToPopupResult();
        public bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES) => Msg.ErrorYesNoB(message, title, result.ToDialogResult());
        public PopupResult ErrorYesNoCancel(string message, string title = "Error") => Msg.ErrorYesNoCancel(message, title).ToPopupResult();
        public bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES) => Msg.ErrorYesNoCancelB(message, title, result.ToDialogResult());
        public void FocusReplaceBar() => App.MainForm?.ReplaceBox.Focus();
        public void FocusSearchBar() => App.MainForm?.SearchBox.Focus();
        public PopupResult InfoOk(string message, string title = "Info") => Msg.InfoOk(message, title).ToPopupResult();
        public PopupResult InfoOkCancel(string message, string title = "Info") => Msg.InfoOkCancel(message, title).ToPopupResult();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => Msg.InfoOkCancelB(message, title, result.ToDialogResult());
        public PopupResult InfoYesNo(string message, string title = "Info") => Msg.InfoYesNo(message, title).ToPopupResult();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => Msg.InfoYesNoB(message, title, result.ToDialogResult());
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => Msg.InfoYesNoCancel(message, title).ToPopupResult();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => Msg.InfoYesNoCancelB(message, title, result.ToDialogResult());
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public void SetReplaceMenuInVisible()
        {
            _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the replacemenu invisible");
            App.MainForm.ReplaceAllButton.Visible = App.MainForm.ReplaceButton.Visible = App.MainForm.ReplaceBox.Visible = false;
        }
        public void SetReplaceMenuVisible()
        {
            _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the replacemenu visible");
            App.MainForm.ReplaceAllButton.Visible = App.MainForm.ReplaceButton.Visible = App.MainForm.ReplaceBox.Visible = true;
        }
        public void SetSelectedSearchBarText(int start, int end)
        {
            if (App.MainForm == null) return;
            if (start > end) throw new ArgumentOutOfRangeException(nameof(end), "End has to be after start");
            App.MainForm.SearchBox.SelectionStart = start;
            App.MainForm.SearchBox.SelectionLength = end - start;
        }

        public void SetTitle(string title)
        {
            _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the App title");
            App.MainForm.Text = title;
        }
        public void SignalAppExit() => Application.Exit();
        public void SignalUserEndWait()
        {
            --waitCounter;
            SetWaitCursor();
        }
        public void SignalUserPing() => System.Media.SystemSounds.Exclamation.Play();

        public void SignalUserWait()
        {
            ++waitCounter;
            SetWaitCursor();
        }
        /// <summary>
        /// Does some logic to figure out wether to show or hide the replacing ui
        /// </summary>
        public void ToggleReplaceBar()
        {
            if (!ReplaceBarIsVisible)
            {
                if (SelectedTab.SelectedTranslationBoxText.Length > 0)
                {
                    SearchBarText = SelectedTab.SelectedTranslationBoxText;
                }
                SetReplaceMenuVisible();

                //set focus to most needed text box, search first
                if (SearchBarText.Length > 0) FocusReplaceBar();
                else FocusSearchBar();
            }
            else
            {
                SetReplaceMenuInVisible();
                SelectedTab.FocusTranslationBox();
            }
        }

        public void Update() => App.MainForm?.Update();

        public PopupResult WarningOk(string message, string title = "Warning") => Msg.WarningOk(message, title).ToPopupResult();

        public PopupResult WarningOkCancel(string message, string title = "Warning") => Msg.WarningOkCancel(message, title).ToPopupResult();

        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => Msg.WarningOkCancelB(message, title, result.ToDialogResult());

        public PopupResult WarningYesNo(string message, string title = "Warning") => Msg.WarningYesNo(message, title).ToPopupResult();

        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => Msg.WarningYesNoB(message, title, result.ToDialogResult());

        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => Msg.WarningYesNoCancel(message, title).ToPopupResult();

        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => Msg.WarningYesNoCancelB(message, title, result.ToDialogResult());

        private void SetWaitCursor()
        {
            if (waitCounter < 0) waitCounter = 0;
            if (waitCounter > 0 && !Application.UseWaitCursor) Application.UseWaitCursor = true;
            else if (waitCounter == 0) Application.UseWaitCursor = false;
            App.MainForm?.Invalidate();
        }

        public void SetLanguageHighlights(string[] languages)
        {
            var indices = new int[languages.Length].AsSpan();
            int iterator = 0;
            for (int i = 0; i < languages.Length; i++)
            {
                int index = App.MainForm?.LanguageBox.DropDown.Items.IndexOf(languages[i]) ?? -1;
                if (index >= 0)
                    indices[iterator++] = index;
            }
            App.MainForm?.LanguageBox.DropDown.SetColoredIndices(indices[..iterator].ToArray());
        }

        internal void ClearUserWaitCount()
        {
            waitCounter = 0;
            Application.UseWaitCursor = false;
        }
    }
}
