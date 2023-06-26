using System.Collections.Generic;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ILineList
    {
        ILineItem this[int index] { get; set; }
        int ApprovedCount { get; }
        int Count { get; }
        List<int> SearchResults { get; }
        int SelectedIndex { get; set; }
        ILineItem SelectedLineItem { get; set; }
        List<int> TranslationSimilarToTemplate { get; }

        void Add(string iD, bool lineIsApproved);
        void AddLineItem(ILineItem item);
        void ApproveItem(int index);
        void Clear();
        void FreezeLayout();
        bool GetApprovalState(int index);
        void RemoveLineItem(ILineItem item);
        void SelectIndex(int index);
        void SetApprovalState(int index, bool isApproved);
        void UnapproveItem(int index);
        void UnFreezeLayout();
    }
}