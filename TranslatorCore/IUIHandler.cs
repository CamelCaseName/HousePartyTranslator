﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Translator.Core;
using Translator.UICompatibilityLayer.StubImpls;

namespace Translator.UICompatibilityLayer
{
    public enum PopupResult
    {
        NONE,
        OK,
        YES,
        NO,
        CANCEL,
        ABORT,
        CONTINUE,
        IGNORE
    }

    public interface IFileDialog
    {
        string FileName { get; set; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }
        string SelectedPath { get; }
        string Title { get; set; }
        PopupResult ShowDialog();
    }
    public interface ISaveFileDialog
    {
        string FileName { get; set; }
        string InitialDirectory { get; set; }
        string Title { get; set; }
        string Extension { get; set; }
        bool PromptCreate { get; set; }
        bool PromptOverwrite { get; set; }

        PopupResult ShowDialog();
    }
    public interface IFolderDialog
    {
        string SelectedFolderPath { get; set; }
        string Text { get; set; }
        PopupResult ShowDialog();
    }

    public interface ILineItem
    {
        public bool IsApproved { get; set; }
        public bool IsSearchResult { get; set; }
        public bool IsTranslated { get; set; }
        EventHandler OnClick { get; init; }
        public string Text { get; init; }
        void Approve();
        void Unapprove();
    }

    public interface IMenuItem
    {
        EventHandler OnClick { get; init; }
        public string Text { get; set; }
    }

    public interface ITab<T> where T : class, ILineItem, new()
    {
        public int LineCount => Lines.Count;
        string Text { get; set; }
        bool IsApproveButtonFocused { get; }

        /// <summary>
        /// Contains the ids of strings similar to the original template
        /// </summary>
        List<string> SimilarStringsToEnglish { get; }

        void Dispose();

        #region list of translations
        void ClearLines();

        T AtIndex(int index);

        LineList<T> Lines { get; set; }
        int SelectedLineIndex { get; }
        T SelectedLineItem { get; }
        string SelectedLine { get { return SelectedLineItem.Text; } }

        bool IsTranslationBoxFocused { get; }
        bool IsCommentBoxFocused { get; }
        int ProgressValue { get; set; }

        void SelectLineItem(int index);

        void SelectLineItem(T item);
        void UpdateLines();

        #endregion

        #region translation textbox
        void FocusTranslationBox();

        string TranslationBoxText { get; set; }

        string SelectedTranslationBoxText { get; }

        void SetSelectedTranslationBoxText(int start, int end);
        #endregion

        #region template textbox
        string TemplateBoxText { get; set; }

        string SelectedTemplateBoxText { get; }

        void SetSelectedTemplateBoxText(int start, int end);
        #endregion

        #region comment textbox
        void FocusCommentBox();

        string CommentBoxText { get; set; }

        string[] CommentBoxTextArr { get; set; }

        string SelectedCommentBoxText();
        void SetSelectedCommentBoxText(int start, int end);
        #endregion

        #region line controls
        void ApproveSelectedLine();
        void UnapproveSelectedLine();
        bool ApprovedButtonChecked { get; set; }

        void SetFileInfoText(string info);
        void SetApprovedLabelText(string text);
        void SetCharacterLabelColor(Color lawnGreen);
        void SetCharacterCountLabelText(string text);
        #endregion
    }

    public interface ITabController<T> where T : class, ILineItem, new()
    {
        int SelectedIndex { get; set; }
        ITab<T> SelectedTab { get; set; }
        int TabCount { get; }
        List<ITab<T>> TabPages { get; }
        bool CloseTab(ITab<T> tab);
    }

    public interface ITextBox
    {
        public EventHandler OnClick { get; init; }
        public EventHandler OnTextChanged { get; init; }
        public int SelectionEnd { get; set; }
        public int SelectionStart { get; set; }
        public string Text { get; set; }
        public void Focus();
    }

    public interface IUIHandler<T> where T : class, ILineItem, new()
    {
        #region cursor
        void SignalUserEndWait();

        void SignalUserWait();
        #endregion

        #region message popup
        PopupResult ErrorOk(string message, string title = "Error");

        PopupResult ErrorOkCancel(string message, string title = "Error");

        bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK);

        PopupResult ErrorYesNo(string message, string title = "Error");

        bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES);

        PopupResult ErrorYesNoCancel(string message, string title = "Error");

        bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES);

        PopupResult InfoOk(string message, string title = "Info");
        PopupResult InfoOkCancel(string message, string title = "Info");
        bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK);

        PopupResult InfoYesNo(string message, string title = "Info");
        bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES);

        PopupResult InfoYesNoCancel(string message, string title = "Info");
        bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES);

        PopupResult WarningOk(string message, string title = "Warning");
        PopupResult WarningOkCancel(string message, string title = "Warning");
        bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK);

        PopupResult WarningYesNo(string message, string title = "Warning");
        bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES);

        PopupResult WarningYesNoCancel(string message, string title = "Warning");
        bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES);
        #endregion

        #region main window

        MenuItems FileMenuItems { get; set; }

        void SetTitle(string title);
        #endregion

        #region login user control
        bool Login();
        bool Logout();

        #endregion

        #region search/replace
        void FocusReplaceBar();

        void FocusSearchBar();
        string ReplaceBarText { get; set; }

        string SearchBarText { get; set; }

        void UpdateResults();
        #endregion

        #region menubar

        #endregion

        #region file access/system access
        Type InternalFileDialogType { get; protected set; }
        Type FileDialogType { get => InternalFileDialogType; init => InternalFileDialogType = typeof(NullFileDialog); }
        Type InternalFolderDialogType { get; protected set; }
        Type FolderDialogType { get => InternalFolderDialogType; init => InternalFileDialogType = typeof(NullFolderDialog); }
        Type InternalSaveFileDialogType { get; protected set; }
        Type SaveFileDialogType { get => InternalSaveFileDialogType; init => InternalFileDialogType = typeof(NullSaveFileDialog); }

        CreateTemplateDataDelegate CreateTemplateData { get; set; }

        void ClipboardSetText(string text);

        ITab<T>? CreateNewTab();

        void SignalAppExit();
        void Update();
        void UpdateProgress();
        void SetReplaceMenuVisible();
        void SetReplaceMenuInVisible();
        #endregion

        #region tabs
        ITabController<T> TabControl { get; }
        string Language { get; set; }
        bool ReplaceBarIsVisible { get; }
        #endregion
    }

    public class LineList<T> where T : class, ILineItem, new()
    {
        public readonly List<T> Items = new();

        public int Count => Items.Count;
        public int ApprovedCount { get; internal set; }
        public LineList() : this(new List<T>()) { }

        public LineList(List<T> items, T selectedLineItem, int selectedIndex)
        {
            Items = items;
            SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public LineList(List<T> items)
        {
            Items = items;
            SelectedLineItem = items.Count > 0 ? items[0] : new T();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public T this[int index] { get { return Items[index]; } set { Items[index] = value; } }

        public int SelectedIndex { get { return InternalSelectedIndex; } set { SelectIndex(value); } }
        public T SelectedLineItem { get; set; }
        private int InternalSelectedIndex { get; set; }
        public List<int> SearchResults { get; internal set; } = new();

        public void AddLineItem(T item)
        {
            Items.Add(item);
        }

        public void ApproveItem(int index)
        {
            try
            {
                Items[index].Approve();
                ++ApprovedCount;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Clear()
        {
            Items.Clear();
            ApprovedCount = 0;
        }

        public bool GetApprovalState(int index)
        {
            try
            {
                return Items[index].IsApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void RemoveLineItem(T item)
        {
            if (item.IsApproved) --ApprovedCount;
            Items.Remove(item);
        }

        public void SelectIndex(int index)
        {
            try
            {
                InternalSelectedIndex = index;
                SelectedLineItem = Items[index];
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void SetApprovalState(int index, bool isApproved)
        {
            try
            {
                if (!Items[index].IsApproved && isApproved) ++ApprovedCount;
                Items[index].IsApproved = isApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void UnapproveItem(int index)
        {
            try
            {
                Items[index].Unapprove();
                --ApprovedCount;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Add(string iD, bool lineIsApproved)
        {
            Items.Add(new T() { Text = iD, IsApproved = lineIsApproved });
        }
    }
    public class MenuItems
    {
        public readonly List<IMenuItem> Items = new();
        public int Count { get { return Items.Count; } }

        public IMenuItem this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }
        internal void Insert<MenuItemType>(int v, MenuItemType menuItem) where MenuItemType : class, IMenuItem
        {
            Items.Insert(v, menuItem);
        }
        internal void RemoveAt(int v)
        {
            Items.RemoveAt(v);
        }
    }
}