﻿using System.Collections.Generic;
using System.Drawing;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ITab<TLineItem>
        where TLineItem : class, ILineItem, new()
    {
        int LineCount => Lines.Count;
        string Text { get; set; }
        bool IsApproveButtonFocused { get; }

        /// <summary>
        /// Contains the ids of strings similar to the original template
        /// </summary>
        List<string> SimilarStringsToEnglish { get; }

        void Dispose();

        #region list of translations
        void ClearLines();

        TLineItem AtIndex(int index);

        ILineList<TLineItem> Lines { get; set; }
        int SelectedLineIndex { get; }
        TLineItem SelectedLineItem { get; }
        string SelectedLine { get { return SelectedLineItem.Text; } }

        bool IsTranslationBoxFocused { get; }
        bool IsCommentBoxFocused { get; }
        int SingleProgressValue { get; set; }
        int AllProgressValue { get; set; }

        void SelectLineItem(int index);

        void SelectLineItem(TLineItem item);
        void UpdateLines();

        #endregion

        #region translation textbox
        ITextBox Translation { get; }
        void FocusTranslationBox();

        string TranslationBoxText { get; set; }

        string SelectedTranslationBoxText { get; }

        void SetSelectedTranslationBoxText(int start, int end);
        #endregion

        #region template textbox
        ITextBox Template { get; }
        string TemplateBoxText { get; set; }

        string SelectedTemplateBoxText { get; }

        void SetSelectedTemplateBoxText(int start, int end);
        #endregion

        #region comment textbox
        ITextBox Comments { get; }
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
        void SetCharacterLabelColor(Color color);
        void SetCharacterCountLabelText(string text);
        #endregion
    }
}