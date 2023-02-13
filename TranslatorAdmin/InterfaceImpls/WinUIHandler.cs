using Translator;
using Translator.Core;
using Translator.Helpers;
using Translator.UICompatibilityLayer;
using Translator.Managers;

namespace Translator.InterfaceImpls
{
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	public sealed class WinUIHandler : IUIHandler<WinLineItem, WinTabController, WinTab>
	{
		private int waitCounter = 0;
		public WinUIHandler() { }

		internal WinUIHandler(ITabController<WinLineItem, WinTab> control)
		{
			TabControl = (WinTabController)control;
		}

		public MenuItems FileMenuItems => App.MainForm?.FileToolStripMenuItem.DropDownItems.ToMenuItems() ?? new MenuItems();

		public string ReplaceBarText { get => App.MainForm?.ReplaceBox.Text ?? string.Empty; set { _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the ReplaceBarText"); App.MainForm.ReplaceBox.Text = value; } }

		public string SearchBarText { get => App.MainForm?.SearchBox.Text ?? string.Empty; set { _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the SearchBArText"); App.MainForm.SearchBox.Text = value; } }

		public CreateTemplateFromStoryDelegate CreateTemplateFromStory { get => WinTranslationManager.CreateTemplateFromStory; }

		public WinTabController TabControl { get; } = new();

		public string Language { get => App.MainForm?.LanguageBox.Text ?? string.Empty; set { _ = App.MainForm ?? throw new NullReferenceException("MainForm was null when setting the language"); App.MainForm.LanguageBox.Text = value; } }

		public bool ReplaceBarIsVisible => App.MainForm?.ReplaceAllButton.Visible ?? false && App.MainForm.ReplaceButton.Visible && App.MainForm.ReplaceBox.Visible;

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
		public WinTab? CreateNewTab()
		{
			if (App.MainForm == null) return null;
			return new(App.MainForm);
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
		public void SignalUserWait()
		{
			++waitCounter;
			SetWaitCursor();
		}
		private void SetWaitCursor()
		{
			if (waitCounter > 0 && !Application.UseWaitCursor) Application.UseWaitCursor = true;
			else if (waitCounter == 0) Application.UseWaitCursor = false;
			App.MainForm.Invalidate();
		}
		public void Update() => App.MainForm?.Update();
		public void UpdateTranslationProgressIndicator() => SelectedTab.ProgressbarTranslated.Invalidate();
		public void UpdateResults() => SelectedTab.Lines.Invalidate();

		public PopupResult WarningOk(string message, string title = "Warning") => Msg.WarningOk(message, title).ToPopupResult();
		public PopupResult WarningOkCancel(string message, string title = "Warning") => Msg.WarningOkCancel(message, title).ToPopupResult();
		public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => Msg.WarningOkCancelB(message, title, result.ToDialogResult());
		public PopupResult WarningYesNo(string message, string title = "Warning") => Msg.WarningYesNo(message, title).ToPopupResult();
		public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => Msg.WarningYesNoB(message, title, result.ToDialogResult());
		public PopupResult WarningYesNoCancel(string message, string title = "Warning") => Msg.WarningYesNoCancel(message, title).ToPopupResult();
		public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => Msg.WarningYesNoCancelB(message, title, result.ToDialogResult());
		public void SetFileMenuItems(MenuItems menuItems)
		{
			if (App.MainForm == null) return;
			App.MainForm.FileToolStripMenuItem.DropDownItems.Clear();
			App.MainForm.FileToolStripMenuItem.DropDownItems.AddRangeFix(menuItems.ToToolStripItemCollection(App.MainForm.MainMenu ?? new MenuStrip()));
		}

		public void SetSelectedSearchBarText(int start, int end)
		{
			if (App.MainForm == null) return;
			if (start > end) throw new ArgumentOutOfRangeException(nameof(end), "End has to be after start");
			App.MainForm.SearchBox.SelectionStart = start;
			App.MainForm.SearchBox.SelectionLength = end - start;
		}
	}
}
