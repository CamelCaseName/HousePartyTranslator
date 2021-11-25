using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.Helpers
{
    /// <summary>
    /// Borrowed from https://stackoverflow.com/questions/2130934/how-change-the-color-of-selecteditem-in-checkedlistbox-in-windowsforms
    /// Creates a coloured Rectangle in each element, depending on checked state.
    /// </summary>
    public class ColouredCheckedListBox : CheckedListBox
    {
        public List<int> SearchResults = new List<int>(); //list containing all indices that are part of the search result

        protected new bool DoubleBuffered = true;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            DrawItemEventArgs e2 =
                new DrawItemEventArgs
                (
                    e.Graphics,
                    e.Font,
                    new Rectangle(e.Bounds.Location, e.Bounds.Size),
                    e.Index,
                    e.State,
                    e.ForeColor, //colour yellow if it is part of the search, else colour normally
                    SearchResults.Contains(e.Index) ? Color.DarkOrange : CheckedIndices.Contains(e.Index) ? Color.FromArgb(80, 130, 80) : Color.FromArgb(130, 80, 80)
                );
            base.OnDrawItem(e2);
        }
    }
}
