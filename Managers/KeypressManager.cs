﻿using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// Class that handles all keyboard presses and calls the appropiate methods if a hotkey was detected
    /// </summary>
    static class KeypressManager
    {
        static public void ApprovedButtonChanged()
        {
            TabManager.ActiveTranslationManager.ApprovedButtonHandler();
        }

        static public void AutoTranslate()
        {
            TabManager.ActiveTranslationManager.ReplaceTranslationTranslatedTask(TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex);
        }

        static public void CheckItemChanged()
        {
            TabManager.ActiveTranslationManager.ApproveIfPossible(false);
        }

        static public StoryExplorerForm.StoryExplorer CreateStoryExplorer(bool autoOpen, Form explorerParent)
        {
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            Form.ActiveForm.UseWaitCursor = true;
            bool isStory = translationManager.StoryName.ToLowerInvariant() == translationManager.FileName.ToLowerInvariant();
            StoryExplorerForm.StoryExplorer Explorer = new StoryExplorerForm.StoryExplorer(isStory, autoOpen, translationManager.FileName, translationManager.StoryName, explorerParent);
            if (!Explorer.IsDisposed) Explorer.Show();
            Form.ActiveForm.UseWaitCursor = false;
            translationManager.SetHighlightedNode();
            return Explorer;
        }

        /// <summary>
        /// Detects for hotkeys used, if they are consumed we return true, else false is returned.
        /// </summary>
        /// <param name="msg">Windows message contaning the info on the event.</param>
        /// <param name="keyData">Keydata containing all currently pressed keys.</param>
        /// <returns></returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
        static public bool MainKeyPressHandler(ref Message msg, Keys keyData, DiscordPresenceManager presence, Form parent)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            switch (keyData)
            {
                //handle enter as jumping to first search result if searched something, and focus is not on text editor.
                case (Keys.Enter):
                    return TabManager.ActiveTranslationManager.SelectNextResultIfApplicable();

                //set selected string as search string and place cursor in search box
                case (Keys.Control | Keys.F):
                    if (TabManager.ActiveProperties.TranslationTextBox.SelectedText.Length > 0)
                    {
                        TabManager.ActiveProperties.SearchBox.Text = TabManager.ActiveProperties.TranslationTextBox.SelectedText;
                    }
                    TabManager.ActiveProperties.SearchBox.Focus();
                    return true;

                //search, but also with replacing
                case (Keys.Control | Keys.Shift | Keys.F):
                    TabManager.ActiveTranslationManager.ToggleReplaceUI();
                    return true;

                //save current file
                case (Keys.Control | Keys.S):
                    TabManager.ActiveTranslationManager.SaveFile();
                    return true;

                //save current string
                case (Keys.Control | Keys.Shift | Keys.S):
                    TabManager.ActiveTranslationManager.SaveCurrentString();
                    return true;

                //saves all open tabs
                case (Keys.Alt | Keys.Shift | Keys.S):
                    TabManager.SaveAllTabs();
                    return true;

                //reload currently loaded file
                case (Keys.Control | Keys.R):
                    TabManager.ActiveTranslationManager.ReloadFile();
                    return true;

                //select string above current selection
                case (Keys.Control | Keys.Up):
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex > 0) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex--;
                    return true;

                //select string below current selection
                case (Keys.Control | Keys.Down):
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count - 1) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //switch tab to the left
                case (Keys.Alt | Keys.Left):
                    if (TabManager.TabControl.TabCount > 1) TabManager.SwitchToTab(TabManager.TabControl.SelectedIndex - 1);
                    return true;

                //switch tab to the right
                case (Keys.Alt | Keys.Right):
                    if (TabManager.TabControl.TabCount > 1) TabManager.SwitchToTab(TabManager.TabControl.SelectedIndex + 1);
                    return true;

                //save translation and move down one
                case (Keys.Control | Keys.Enter):
                    TabManager.ActiveTranslationManager.SaveCurrentString();
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count - 1) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //save translation, approve and move down one
                case (Keys.Control | Keys.Shift | Keys.Enter):
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex >= 0) TabManager.ActiveProperties.CheckListBoxLeft.SetItemChecked(TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex, true);
                    else TabManager.ActiveProperties.CheckListBoxLeft.SetItemChecked(0, true);
                    if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex < TabManager.ActiveProperties.CheckListBoxLeft.Items.Count - 1) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex++;
                    return true;

                //ripple delete all chars to the right of the cursor to the next nonalphanumerical char
                case (Keys.Control | Keys.Delete):
                    return TabManager.ActiveTranslationManager.DeleteCharactersInText(false);

                //ripple delete all alphanumerical chars to the left of the cursor
                case (Keys.Control | Keys.Back):
                    return TabManager.ActiveTranslationManager.DeleteCharactersInText(true);

                //move cursor to the left, clinging to words
                case (Keys.Control | Keys.Left):
                    return TabManager.ActiveTranslationManager.MoveCursorInText(true);

                //move cursor to the right, clinging to words
                case (Keys.Control | Keys.Right):
                    return TabManager.ActiveTranslationManager.MoveCursorInText(false);

                case Keys.Control | Keys.O:
                    OpenNew(presence);
                    return true;

                case Keys.Control | Keys.Shift | Keys.O:
                    OpenNewTab(presence);
                    return true;

                case Keys.Alt | Keys.Shift | Keys.O:
                    OpenAll(presence);
                    return true;

                case Keys.Control | Keys.E:
                    CreateStoryExplorer(true, parent);
                    return true;

                case Keys.Control | Keys.T:
                    CreateStoryExplorer(false, parent);
                    return true;

                case Keys.Control | Keys.P:
                    ShowSettings();
                    return true;

                default:
                    return false;
            }
        }

        static public void OpenAll(DiscordPresenceManager presenceManager)
        {
            //opne the story in tabs
            TabManager.OpenAllTabs();

            //update presence and recents
            presenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        static public void OpenContextMenu(ContextMenuStrip context, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = TabManager.ActiveProperties.CheckListBoxLeft.IndexFromPoint(e.Location);
                if (TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex <= 0) TabManager.ActiveProperties.CheckListBoxLeft.SelectedIndex = 0;
                context.Show();
            }
        }

        static public void OpenNew(DiscordPresenceManager presenceManager)
        {
            //get currently active translationmanager
            TranslationManager translationManager = TabManager.ActiveTranslationManager;
            translationManager.LoadFileIntoProgram();
            translationManager.SetLanguage();
            //update tab name
            if (translationManager.FileName.Length > 0) TabManager.TabControl.SelectedTab.Text = translationManager.FileName;

            //update presence and recents
            presenceManager.Update(translationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        static public void OpenNewTab(DiscordPresenceManager presenceManager)
        {
            TabManager.OpenNewTab();

            //update presence and recents
            presenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        static public void SelectedItemChanged()
        {
            TabManager.ActiveTranslationManager.PopulateTextBoxes();
        }

        static public void SelectedLanguageChanged()
        {
            TabManager.ActiveTranslationManager.SetLanguage();
        }

        static public void SelectedTabChanged(DiscordPresenceManager presenceManager)
        {
            TabManager.OnSwitchTabs();
            //update tabs
            if (TabManager.ActiveTranslationManager != null) presenceManager.Update(TabManager.ActiveTranslationManager.StoryName, TabManager.ActiveTranslationManager.FileName);
        }

        static public void ShowSettings()
        {
            SettingsForm.SettingsForm settings = new SettingsForm.SettingsForm();
            if (!settings.IsDisposed) settings.Show();
        }

        static public void TranslationTextChanged()
        {
            TabManager.ActiveTranslationManager.UpdateTranslationString();
        }
    }
}