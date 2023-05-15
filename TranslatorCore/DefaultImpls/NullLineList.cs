using System;
using System.Collections.Generic;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.DefaultImpls
{
    public class NullLineList<TLineItem> : ILineList<TLineItem>
        where TLineItem : class, ILineItem, new()
    {
        public readonly List<TLineItem> Items = new();

        public int Count => Items.Count;
        public int ApprovedCount { get; internal set; }
        public NullLineList() : this(new List<TLineItem>()) { }

        public NullLineList(List<TLineItem> items, TLineItem selectedLineItem, int selectedIndex)
        {
            Items = items;
            SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public NullLineList(List<TLineItem> items)
        {
            Items = items;
            SelectedLineItem = items.Count > 0 ? items[0] : new TLineItem();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public TLineItem this[int index] { get { return Items[index]; } set { Items[index] = value; } }

        public int SelectedIndex { get { return InternalSelectedIndex; } set { SelectIndex(value); } }
        public TLineItem SelectedLineItem { get; set; }
        private int InternalSelectedIndex { get; set; }
        public List<string> SearchResults { get; internal set; } = new();
        public List<string> TranslationSimilarToTemplate { get; internal set; } = new();

        public void AddLineItem(TLineItem item)
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

        public void RemoveLineItem(TLineItem item)
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
            Items.Add(new TLineItem() { Text = iD, IsApproved = lineIsApproved });
        }

        public void FreezeLayout() { }
        public void UnFreezeLayout() { }
    }
}