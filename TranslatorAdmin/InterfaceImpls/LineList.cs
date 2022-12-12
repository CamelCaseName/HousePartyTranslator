using Translator.Helpers;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    public class LineList : ColouredCheckedListBox, ILineList<WinLineItem>
    {
        public int Count => Items.Count;
        public int ApprovedCount { get; internal set; }
        public LineList() : this(new List<WinLineItem>()) { }

        public LineList(List<WinLineItem> items, WinLineItem selectedLineItem, int selectedIndex)
        {
            Items.Clear();
            Items.AddRange((ListBox.ObjectCollection)items.Cast<object>());
            ((ILineList<WinLineItem>)this).SelectedLineItem = selectedLineItem;
            SelectedIndex = selectedIndex;
        }

        public LineList(List<WinLineItem> items)
        {
            Items.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                Items.Add(items[i]);
            }
            ((ILineList<WinLineItem>)this).SelectedLineItem = items.Count > 0 ? items[0] : new WinLineItem();
            SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        public WinLineItem this[int index] { get { return (WinLineItem)Items[index]; } set { Items[index] = value; } }

        WinLineItem ILineList<WinLineItem>.SelectedLineItem { get; set; } = new WinLineItem();
        public List<int> TranslationSimilarToTemplate { get; internal set; } = new();

        //todo make better lol
        List<string> ILineList<WinLineItem>.SearchResults => SearchResults;

        List<string> ILineList<WinLineItem>.TranslationSimilarToTemplate { get; } = new();

        WinLineItem ILineList<WinLineItem>.this[int index] { get => (WinLineItem)Items[index]; set => Items[index] = value; }

        void ILineList<WinLineItem>.AddLineItem(WinLineItem item)
        {
            Items.Add(item);
        }

        public void ApproveItem(int index)
        {
            try
            {
                ((WinLineItem)Items[index]).Approve();
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
                return ((WinLineItem)Items[index]).IsApproved;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void RemoveLineItem(WinLineItem item)
        {
            if (item.IsApproved) --ApprovedCount;
            Items.Remove(item);
        }

        public void SelectIndex(int index)
        {
            try
            {
                ((ILineList<WinLineItem>)this).SelectedLineItem = ((WinLineItem)Items[index]);
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
                if (!((WinLineItem)Items[index]).IsApproved && isApproved) ++ApprovedCount;
                ((WinLineItem)Items[index]).IsApproved = isApproved;
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
                --ApprovedCount;
            }
            catch
            {
                throw new IndexOutOfRangeException("Given index was too big for the array");
            }
        }

        public void Add(string iD, bool lineIsApproved)
        {
            Items.Add(new WinLineItem() { Text = iD, IsApproved = lineIsApproved });
        }
    }
}
