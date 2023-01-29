using Translator.UICompatibilityLayer;
using Translator;
using InputHandler = Translator.Core.InputHandler<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;
using TabManager = Translator.Core.TabManager<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;

namespace Translator.Managers
{
	/// <summary>
	/// Class that handles all keyboard presses and calls the appropiate methods if a hotkey was detected
	/// </summary>
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	internal static class WindowsKeypressManager
	{
		public static void OpenContextMenu(ContextMenuStrip context, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && TabManager.UI != null)
			{
				if (TabManager.UI.SelectedTab.Lines.Count > 0)
				{
					TabManager.UI.SelectedTab.Lines.SelectedIndex = TabManager.UI.SelectedTab.Lines.IndexFromPoint(e.Location);
					if (TabManager.UI.SelectedTab.Lines.SelectedIndex <= 0) TabManager.UI.SelectedTab.Lines.SelectedIndex = 0;
					context.Show();
				}
			}
		}

		public static void SelectedTabChanged(DiscordPresenceManager? presenceManager)
		{
			//update presence
			if (TabManager.ActiveTranslationManager != null) presenceManager?.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
		}

		public static void ShowSettings()
		{
			var settings = new SettingsForm.SettingsForm();
			if (!settings.IsDisposed) settings.Show();
		}

		/// <summary>
		/// Call this after performing base.WndProc, but before returning in the overriden form WndProc
		/// </summary>
		public static void WinTextChangedCallback(Form parent, int selectedIndex)
		{
			if (parent.ActiveControl == null) return;
			if (parent.ActiveControl.GetType().IsAssignableFrom(typeof(ITextBox)))
			{
				InputHandler.TextChangedCallback((ITextBox)parent.ActiveControl, selectedIndex);
			}
		}

		/// <summary>
		/// Detects for hotkeys used, if they are consumed we return true, else false is returned. Call TextChangedCallback if this returned false and the base.WndProc has finished to call back on text changes.
		/// </summary>
		/// <param name="msg">Windows message contaning the info on the event.</param>
		/// <param name="keyData">Keydata containing all currently pressed keys.</param>
		/// <param name="presence"></param>
		/// <param name="parent"></param>
		/// <param name="tokenSource"></param>
		/// <returns></returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
		public static bool MainKeyPressHandler(ref Message msg, Keys keyData, CancellationTokenSource tokenSource)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
		{
			if (TabManager.UI == null || App.MainForm == null) return false;
			switch (keyData)
			{
				//handle enter as jumping to first search result if searched something, and focus is not on text editor.
				case (Keys.Enter):
					return InputHandler.AdvanceSearchResultSelection();

				//set selected string as search string and place cursor in search box
				case (Keys.Control | Keys.F):
					InputHandler.FocusSearch();
					return true;

				//search, but also with replacing
				case (Keys.Control | Keys.Shift | Keys.F):
					InputHandler.ToggleReplaceUI();
					return true;

				//save current file
				case (Keys.Control | Keys.S):
					InputHandler.SaveFile();
					return true;

				//save current string
				case (Keys.Control | Keys.Shift | Keys.S):
					InputHandler.SaveSelectedString();
					return true;

				//saves all open tabs
				case (Keys.Alt | Keys.Shift | Keys.S):
					InputHandler.SaveAllTabs();
					return true;

				//reload currently loaded file
				case (Keys.Control | Keys.R):
					InputHandler.ReloadFile();
					return true;

				//select string above current selection
				case (Keys.Control | Keys.Up):
					InputHandler.MoveLineSelectionUp();
					return true;

				//select string below current selection
				case (Keys.Control | Keys.Down):
					InputHandler.MoveLineSelectionDown();
					return true;

				//switch tab to the left
				case (Keys.Alt | Keys.Left):
					InputHandler.SelectTabLeft();
					return true;

				//switch tab to the right
				case (Keys.Alt | Keys.Right):
					InputHandler.SelectTabRight();
					return true;

				//save translation and move down one
				case (Keys.Control | Keys.Enter):
					InputHandler.SaveAndSelectNewLine();
					return true;

				//save translation, approve and move down one
				case (Keys.Control | Keys.Shift | Keys.Enter):
					InputHandler.SaveAndApproveAndSelectNewLine();
					return true;

				//ripple delete all chars to the right of the cursor to the next nonalphanumerical char
				case (Keys.Control | Keys.Delete):
					return App.MainForm.DeleteCharactersInText(false);

				//ripple delete all alphanumerical chars to the left of the cursor
				case (Keys.Control | Keys.Back):
					return App.MainForm.DeleteCharactersInText(true);

				//move cursor to the left, clinging to words
				case (Keys.Control | Keys.Left):
					return App.MainForm.MoveCursorInText(true);

				//move cursor to the right, clinging to words
				case (Keys.Control | Keys.Right):
					return App.MainForm.MoveCursorInText(false);

				case Keys.Control | Keys.O:
					InputHandler.OpenNew();
					return true;

				case Keys.Control | Keys.Z:
					InputHandler.Undo();
					return true;

				case Keys.Control | Keys.U:
					InputHandler.Redo();
					return true;

				case Keys.Control | Keys.Shift | Keys.O:
					InputHandler.OpenNewTab();
					return true;

				case Keys.Alt | Keys.Shift | Keys.O:
					InputHandler.OpenAll();
					return true;

				case Keys.Control | Keys.E:
					_ = Fenster.CreateStoryExplorer(true, tokenSource);
					return true;

				case Keys.Control | Keys.T:
					_ = Fenster.CreateStoryExplorer(false, tokenSource);
					return true;

				case Keys.Control | Keys.P:
					ShowSettings();
					return true;

				default:
					DefaultTextChangePreparation();
					//return false, we dont consume the keypresses, only save a state to monitor for change
					return false;
			}
		}

		private static void DefaultTextChangePreparation()
		{
			if (App.MainForm?.ActiveControl == null || App.MainForm == null) return;
			if (App.MainForm.ActiveControl.GetType().IsAssignableFrom(typeof(ITextBox)))
			{
				InputHandler.PrepareTextChanged((ITextBox)App.MainForm.ActiveControl);
			}
		}
	}
}