using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core;
using Translator.Core.Data;
using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.InterfaceImpls;

namespace Translator.Desktop.UI.Components
{
    [SupportedOSPlatform("windows")]
    public class LineList : ColoredCheckedListBox, ILineList
    {
        protected override void WndProc(ref Message m) => base.WndProc(ref m);
        protected override void OnDrawItem(DrawItemEventArgs e) => base.OnDrawItem(e);
        public int Count => Items.Count;
        public int ApprovedCount { get { return CheckedIndices.Count; } }
        public LineList()
        {
            Items.Clear();
            iDs.Clear();
            SelectedIndex = -1;
        }

        private readonly List<EekStringID> iDs = new();


        List<int> ILineList.SearchResults => SearchResults;

        ILineItem ILineList.this[int index] { get => (WinLineItem)Items[index]; set => Items[index] = value; }

        public void ApproveItem(int index)
        {
            try
            {
                ((WinLineItem)Items[index]).Approve();
                SetItemChecked(index, true);
                _ = SimilarStringsToEnglish.Remove(((WinLineItem)Items[index]).ToString());
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Clear()
        {
            Items.Clear();
            iDs.Clear();
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

        public void SetApprovalState(int index, bool isApproved)
        {
            try
            {
                ((WinLineItem)Items[index]).IsApproved = isApproved;
                SetItemChecked(index, isApproved);
                if (!isApproved)
                    TabManager.ActiveTranslationManager.UpdateSimilarityMarking(iDs[index]);
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
                TabManager.ActiveTranslationManager.UpdateSimilarityMarking(iDs[index]);
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Add(EekStringID iD, bool lineIsApproved)
        {
            _ = Items.Add(new WinLineItem() { Text = iD.ID, IsApproved = lineIsApproved, ID = iD }, lineIsApproved);
            iDs.Add(iD);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            ((WinLineItem)Items[ice.Index]).IsApproved = ice.NewValue == CheckState.Checked;
            base.OnItemCheck(ice);
        }

        public void FreezeLayout()
        {
            SuspendLayout();
            SimilarStringsToEnglish.CollectionChanged -= UpdateOnCountChange;
        }

        public void UnFreezeLayout()
        {
            ResumeLayout();
            SimilarStringsToEnglish.CollectionChanged += UpdateOnCountChange;
        }

        private void UpdateOnCountChange(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Invoke(() =>
            {
                Invalidate();
                Update();
            });
        }
    }
}
