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
                    e.ForeColor,
                    CheckedIndices.Contains(e.Index) ? Color.DarkGreen : Color.DarkRed
                );

            base.OnDrawItem(e2);
        }
    }
}
