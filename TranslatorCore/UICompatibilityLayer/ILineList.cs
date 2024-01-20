using System.Collections.Generic;
using Translator.Core.Data;

namespace Translator.Core.UICompatibilityLayer
{
    public interface ILineList
    {
        ILineItem this[int index] { get; set; }
        int ApprovedCount { get; }
        int Count { get; }
        List<int> SearchResults { get; }
        int SelectedIndex { get; set; }

        void Add(EekStringID iD, bool lineIsApproved);
        void ApproveItem(int index);
        void Clear();
        void FreezeLayout();
        bool GetApprovalState(int index);
        void SetApprovalState(int index, bool isApproved);
        void UnapproveItem(int index);
        void UnFreezeLayout();
    }
}