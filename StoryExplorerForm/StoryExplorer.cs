using HousePartyTranslator.Helpers;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private readonly ContextProvider Context;
        //readonly Pen NodePen = new Pen(Color.Aquamarine);
        //readonly Pen EdgePen = new Pen(Color.Coral);
        private readonly bool ReadyToDraw = false;
        private float TranslationOffsetX = 0;
        private float TranslationOffsetY = 0;
        private float GraphScale = 1;
        private Point LastMousePostion = MousePosition;
        private Point CurrentMousePosition = MousePosition;

        public StoryExplorer(bool IsStory)
        {
            Cursor = Cursors.WaitCursor;
            InitializeComponent();
            Context = new ContextProvider("", IsStory, false);

            //add custom paint event handler to draw all nodes and edges
            Paint += new PaintEventHandler(DrawNodes);

            //parse story, and not get cancelled xD
            if (Context.ParseFile() || Context.GotCancelled)
            {

                //allow paint handler to draw
                ReadyToDraw = true;
                Cursor = Cursors.Default;
            }
            else
            {
                //else we quit
                Close();
            }
        }

        private void DrawNodes(object sender, PaintEventArgs e)
        {
            if (ReadyToDraw)
            {
                //calculate offfset if mouse button is pressed
                //the values are only updated if the button is pressed
                TranslationOffsetX -= LastMousePostion.X - CurrentMousePosition.X;
                TranslationOffsetY -= LastMousePostion.Y - CurrentMousePosition.Y;


                Graphics graphics = e.Graphics;
                //go on displaying graph
                foreach (Node node in Context.GetNodes())
                {
                    //convert coordinates
                    GraphToScreen((node.Position.X - 3) * GraphScale + TranslationOffsetX, (node.Position.Y - 3) * GraphScale + TranslationOffsetY, out float screenX, out float screenY);

                    //draw ndoe
                    graphics.FillEllipse(Brushes.Aquamarine, screenX, screenY, 6, 6);

                    //draw edges
                    foreach (Node child in node.ChildNodes)
                    {
                        graphics.DrawLine(Pens.Coral, (node.Position.X) * GraphScale + TranslationOffsetX, (node.Position.Y) * GraphScale + TranslationOffsetY, (child.Position.X) * GraphScale + TranslationOffsetX, (child.Position.Y) * GraphScale + TranslationOffsetY);
                    }
                }
            }
        }


        //converts graph coordinates into the corresponding screen coordinates, taking into account all transformations/zoom
        private void GraphToScreen(float graphX, float graphY, out float screenX, out float screenY)
        {
            screenX = graphX;
            screenY = graphY;
        }

        //converts screen coordinates into the corresponding graph coordinates, taking into account all transformations/zoom
        private void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX;
            graphY = screenY;
        }


        private void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //save last, add new
                LastMousePostion = CurrentMousePosition;
                CurrentMousePosition = e.Location;
                Invalidate();
            }
            else
            {
                //reset last
                //WHEEL_DELTA = 120, as per windows documentation
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
                GraphScale += e.Delta / 120 * 0.05f;
                if (GraphScale <= 0) GraphScale = 0.05f;
                //LastMousePostion = CurrentMousePosition;
            }
        }
    }
}
