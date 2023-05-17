using System;
using System.Collections.Generic;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.DefaultImpls
{
    public class NullLineList : ILineList
    {
        public readonly List<ILineItem> Items = new();

        public int Count => Items.Count;
        public int ApprovedCount { get; internal set; }
        public NullLineList() : this(new List<ILineItem>()) { }

        public NullLineList(List<ILineItem> items, ILineItem selectedLineItem, int selectedIndex)
        {
            Items = items;
            SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public NullLineList(List<ILineItem> items)
        {
            Items = items;
            SelectedLineItem = items.Count > 0 ? items[0] : new DefaultLineItem();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public ILineItem this[int index] { get { return Items[index]; } set { Items[index] = value; } }

        public int SelectedIndex { get { return InternalSelectedIndex; } set { SelectIndex(value); } }
        public ILineItem SelectedLineItem { get; set; }
        private int InternalSelectedIndex { get; set; }
        public List<string> SearchResults { get; internal set; } = new();
        public List<string> TranslationSimilarToTemplate { get; internal set; } = new();

        public void AddLineItem(ILineItem item)
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

        public void RemoveLineItem(ILineItem item)
        {
            if (item.IsApproved) --ApprovedCount;
            _ = Items.Remove(item);
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
            Items.Add(new DefaultLineItem() { Text = iD, IsApproved = lineIsApproved });
        }

        public void FreezeLayout() { }
        public void UnFreezeLayout() { }
    }
}