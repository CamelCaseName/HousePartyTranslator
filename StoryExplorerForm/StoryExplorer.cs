using HousePartyTranslator.Helpers;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private bool CurrentlyInPan = false;
        private float AfterZoomMouseX = 0f;
        private float AfterZoomMouseY = 0f;
        private float BeforeZoomMouseX = 0f;
        private float BeforeZoomMouseY = 0f;
        private float OffsetX = 0f;
        private float OffsetY = 0f;
        private float Scaling = 1f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private readonly bool ReadyToDraw = false;
        private readonly ContextProvider Context;

        public StoryExplorer(bool IsStory)
        {
            Cursor = Cursors.WaitCursor;
            //set offset
            OffsetX = -ClientRectangle.X / 2;
            OffsetY = -ClientRectangle.Y / 2;

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
                Graphics graphics = e.Graphics;
                //go on displaying graph
                foreach (Node node in Context.GetNodes())
                {
                    //convert coordinates
                    GraphToScreen(node.Position.X - 5, node.Position.Y - 5, out float screenX, out float screenY);

                    //draw ndoe
                    graphics.FillEllipse(Brushes.Aquamarine, screenX, screenY, 10, 10);

                    //draw edges
                    foreach (Node child in node.ChildNodes)
                    {
                        //convert child coordinates
                        GraphToScreen(child.Position.X, child.Position.Y, out float childScreenX, out float childScreenY);

                        //draw edge to child
                        graphics.DrawLine(Pens.Coral, screenX, screenY, childScreenX, childScreenY);
                    }
                }
            }
        }

        //better scaling thanks to the one lone coder
        //https://www.youtube.com/watch?v=ZQ8qtAizis4

        //converts graph coordinates into the corresponding screen coordinates, taking into account all transformations/zoom
        private void GraphToScreen(float graphX, float graphY, out float screenX, out float screenY)
        {
            screenX = (graphX - OffsetX) * Scaling;
            screenY = (graphY - OffsetY) * Scaling;
        }

        //converts screen coordinates into the corresponding graph coordinates, taking into account all transformations/zoom
        private void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling + OffsetX;
            graphY = screenY / Scaling + OffsetY;
        }


        private void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            //start of pan
            if (e.Button == MouseButtons.Middle && !CurrentlyInPan)
            {
                CurrentlyInPan = true;
                //get current position in screen coordinates when we start to pan
                StartPanOffsetX = e.X;
                StartPanOffsetY = e.Y;
            }
            //in pan
            else if (e.Button == MouseButtons.Middle && CurrentlyInPan)
            {
                //update opffset by the difference in screen coordinates we travelled so far
                OffsetX -= (e.X - StartPanOffsetX) / Scaling;
                OffsetY -= (e.Y - StartPanOffsetY) / Scaling;

                //replace old start by new start coordinates so we only look at one interval,
                //and do not accumulate the change in position
                StartPanOffsetX = e.X;
                StartPanOffsetY = e.Y;

                //redraw
                Invalidate();
            }
            //end of pan
            else if (e.Button != MouseButtons.Middle && CurrentlyInPan)
            {
                CurrentlyInPan = false;
            }
            //everything else, scrolling for example
            else if(e.Delta != 0)
            {
                //get last mouse position in world space before the zoom so we can
                //offset back by the distance in world space we got shifted by zooming
                ScreenToGraph(e.X, e.Y, out BeforeZoomMouseX, out BeforeZoomMouseY);
                //reset last
                //WHEEL_DELTA = 120, as per windows documentation
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
                if (e.Delta > 0)
                {
                    Scaling *= 1.1f;
                }
                else if (e.Delta < 0)
                {
                    Scaling *= 0.9f;
                }

                //capture mouse coordinates in world space again so we can calculate the offset cause by zooming and compensate
                ScreenToGraph(e.X, e.Y, out AfterZoomMouseX, out AfterZoomMouseY);

                //update pan offset by the distance caused by zooming
                OffsetX += BeforeZoomMouseX - AfterZoomMouseX;
                OffsetY += BeforeZoomMouseY - AfterZoomMouseY;

                //redraw
                Invalidate();
            }
        }
    }
}
