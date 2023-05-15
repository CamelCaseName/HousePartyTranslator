using System.Collections.Generic;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ILineList<TLineItem>
        where TLineItem : class, ILineItem, new()
    {
        TLineItem this[int index] { get; set; }
        int ApprovedCount { get; }
        int Count { get; }
        List<string> SearchResults { get; }
        int SelectedIndex { get; set; }
        TLineItem SelectedLineItem { get; set; }
        List<string> TranslationSimilarToTemplate { get; }

        void Add(string iD, bool lineIsApproved);
        void AddLineItem(TLineItem item);
        void ApproveItem(int index);
        void Clear();
        void FreezeLayout();
        bool GetApprovalState(int index);
        void RemoveLineItem(TLineItem item);
        void SelectIndex(int index);
        void SetApprovalState(int index, bool isApproved);
        void UnapproveItem(int index);
        void UnFreezeLayout();
    }
}