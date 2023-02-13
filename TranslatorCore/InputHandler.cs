using System;
using Translator.UICompatibilityLayer;

namespace Translator.Core
{
    public class InputHandler<TLineItem, TUIHandler, TTabController, TTab>
        where TLineItem : class, ILineItem, new()
        where TUIHandler : class, IUIHandler<TLineItem, TTabController, TTab>, new()
        where TTabController : class, ITabController<TLineItem, TTab>, new()
        where TTab : class, ITab<TLineItem>, new()
    {
        private static ITextBox? lastChangedTextBox;
        private static string? lastText;
        private static int lastIndex = Settings.Default.RecentIndex;

        public static void ApprovedButtonChanged()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.ApprovedButtonHandler();
        }

        public static void AutoTranslate()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.RequestedAutomaticTranslation();
        }

        public static void CheckItemChanged()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.ApproveIfPossible(false);
        }

        public static void Redo() => History.Redo();

        public static void Undo() => History.Undo();

        public static void OnSwitchTabs() => TabManager<TLineItem, TUIHandler, TTabController, TTab>.OnSwitchTabs();

        public static void SaveAndApproveAndSelectNewLine()
        {
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex >= 0)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.ApproveItem(TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex);
            else
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.ApproveItem(0);
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex < TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.LineCount - 1)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void SaveAndSelectNewLine()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.SaveCurrentString();
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex < TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.Count - 1)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void SelectTabRight()
        {
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.TabCount > 1)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.SwitchToTab(TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTabIndex + 1);
        }

        public static void SelectTabLeft()
        {
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.TabCount > 1)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.SwitchToTab(TabManager<TLineItem, TUIHandler, TTabController, TTab>.SelectedTabIndex - 1);
        }

        public static void MoveLineSelectionDown()
        {
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex < TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.LineCount - 1)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex++;
        }

        public static void MoveLineSelectionUp()
        {
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex > 0)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex--;
        }

        public static void ReloadFile() =>
                            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.ReloadFile();

        public static void SaveAllTabs() =>
                            _ = TabManager<TLineItem, TUIHandler, TTabController, TTab>.SaveAllTabs();

        public static void SaveSelectedString() =>
                            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.SaveCurrentString();

        public static void SaveFile() => TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.SaveFile();

        public static void FocusSearch()
        {
            //we have something selected we want to search for
            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.TabControl.SelectedTab.SelectedTranslationBoxText.Length > 0)
            {
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SearchBarText = TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.TranslationBoxText;
            }
            //we dont have anything selected, enter empty query
            else
            {
                const string query = "Search here";
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SearchBarText = query;
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SetSelectedSearchBarText(0, query.Length);
            }
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.FocusSearchBar();
        }

        public static bool AdvanceSearchResultSelection() => TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.SelectNextResultIfApplicable();

        /// <summary>
        /// call this before the text changed
        /// </summary>
        /// <param name="textBox"></param>
        public static void PrepareTextChanged(ITextBox textBox)
        {
            lastChangedTextBox = textBox;
            lastText = lastChangedTextBox.Text;
        }

        /// <summary>
        /// Call this after the text has changed
        /// </summary>
        public static void TextChangedCallback(ITextBox text, int selectedIndex)
        {
            if (text == lastChangedTextBox
                && lastText != lastChangedTextBox?.Text
                && lastIndex == selectedIndex
                && lastChangedTextBox != null
                && lastText != null)
            {
                History.AddAction(new TextChanged(
                    lastChangedTextBox,
                    lastText,
                    lastChangedTextBox.Text,
                    TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.FileName,
                    TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.StoryName));
            }
        }

        public static void OpenAll()
        {
            //opne the story in tabs
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.OpenAllTabs();
        }

        public static void OpenNew()
        {
            //get currently active translationmanager
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.OpenNewFile();
        }

        public static void OpenNewTab()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.OpenNewTab();
        }

        public static void SelectedItemChanged(ILineList<TLineItem> listBox)
        {
            if (lastIndex >= 0 && listBox.SelectedIndex >= 0)
            {
                if (History.Peek().FileName == TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.FileName && History.Peek().StoryName == TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.StoryName)
                    History.AddAction(new SelectedLineChanged<TLineItem>(listBox, lastIndex, listBox.SelectedIndex, TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.FileName, TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.StoryName));
                else
                    History.AddAction(new SelectedLineChanged<TLineItem>(listBox, 0, listBox.SelectedIndex, TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.FileName, TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.StoryName));
            }
            lastIndex = listBox.SelectedIndex;
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.PopulateTextBoxes();
        }

        public static void SelectedLanguageChanged()
        {
            TranslationManager<TLineItem, TUIHandler, TTabController, TTab>.SetLanguage();
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.ReloadFile();
        }

        public static void ToggleReplaceUI()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.ToggleReplaceUI();
        }

        public static void TranslationTextChanged()
        {
            TabManager<TLineItem, TUIHandler, TTabController, TTab>.ActiveTranslationManager.UpdateTranslationString();
        }

        public static void SaveAndApproveLine()
        {

            if (TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex >= 0)
                TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.ApproveItem(TabManager<TLineItem, TUIHandler, TTabController, TTab>.UI.SelectedTab.Lines.SelectedIndex);
        }
    }
}
