using System.Windows.Forms;

namespace Translator.Desktop.UI.Components
{
    internal class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item.GetType() != typeof(SearchToolStripTextBox))
            {
                base.OnRenderItemText(e);
            }
            else
            {
                var item = ((SearchToolStripTextBox)e.Item);
                if (item.TotalSearchResults > 0)
                {
                    var size = TextRenderer.MeasureText(item.Counter, item.Font);
                    var pos = new System.Drawing.Point(
                        item.ContentRectangle.Right - size.Width + item.Bounds.Left,
                        item.ContentRectangle.Top - ((item.ContentRectangle.Height - size.Height) / 2) + item.Bounds.Top);
                    TextRenderer.DrawText(
                            e.Graphics,
                            item.Counter,
                            item.Font,
                            pos,
                            item.ForeColor,
                            item.BackColor
                        );
                }
            }
        }
    }
}
