﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.InterfaceImpls;

namespace Translator.Desktop.UI.Components
{
    public class LineList : ColouredCheckedListBox, ILineList
    {
        protected override void WndProc(ref Message m) => base.WndProc(ref m);
        protected override void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);
        public int Count => Items.Count;
        public int ApprovedCount { get { return CheckedIndices.Count; } }
        public LineList() : this(new List<WinLineItem>()) { }

        public LineList(List<WinLineItem> items, WinLineItem selectedLineItem, int selectedIndex)
        {
            Items.Clear();
            var objects = new object[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                objects[i] = items[i];
            }
            ListBox.ObjectCollection collection = new(this, objects);
            Items.AddRange(collection);
            ((ILineList)this).SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public LineList(List<WinLineItem> items)
        {
            Items.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                _ = Items.Add(items[i]);
            }
            ((ILineList)this).SelectedLineItem = items.Count > 0 ? items[0] : new WinLineItem();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public WinLineItem this[int index] { get { return (WinLineItem)Items[index]; } set { Items[index] = value; } }

        ILineItem ILineList.SelectedLineItem { get; set; } = new WinLineItem();

        public List<int> TranslationSimilarToTemplate { get; internal set; } = new();

        List<string> ILineList.SearchResults => SearchResults;

        List<string> ILineList.TranslationSimilarToTemplate { get; } = new();

        ILineItem ILineList.this[int index] { get => (WinLineItem)Items[index]; set => Items[index] = value; }

        void ILineList.AddLineItem(ILineItem item)
        {
            _ = Items.Add(item);
        }

        public void ApproveItem(int index)
        {
            try
            {
                ((WinLineItem)Items[index]).Approve();
                SetItemChecked(index, true);
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool GetApprovalState(int index)
        {
            try
            {
                return ((WinLineItem)Items[index]).IsApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void RemoveLineItem(ILineItem item)
        {
            Items.Remove(item);
        }

        public void SelectIndex(int index)
        {
            try
            {
                ((ILineList)this).SelectedLineItem = (WinLineItem)Items[index];
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
                ((WinLineItem)Items[index]).IsApproved = isApproved;
                SetItemChecked(index, isApproved);
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
                ((WinLineItem)Items[index]).Unapprove();
                SetItemChecked(index, false);
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Add(string iD, bool lineIsApproved)
        {
            _ = Items.Add(new WinLineItem() { Text = iD, IsApproved = lineIsApproved }, lineIsApproved);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            ((WinLineItem)Items[ice.Index]).IsApproved = ice.NewValue == CheckState.Checked;
            base.OnItemCheck(ice);
        }

        public void FreezeLayout() => SuspendLayout();

        public void UnFreezeLayout() => ResumeLayout();
    }
}