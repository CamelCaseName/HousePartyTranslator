using HousePartyTranslator.Helpers;
using System;
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
        private float Scaling = 0.3f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        public bool ReadyToDraw = false;
        private readonly ContextProvider Context;
        private readonly Bitmap GraphBitmap;
        private const int BitmapEdgeLength = 4000;
        private const int Nodesize = 16;
        public Node NodeToHighlight;

        int selector = -1;

        public StoryExplorer(bool IsStory, bool AutoLoad)
        {
            Cursor = Cursors.WaitCursor;
            //set offset
            OffsetX = -BitmapEdgeLength / 2;
            OffsetY = -BitmapEdgeLength / 2;

            InitializeComponent();

            Context = new ContextProvider("", IsStory, AutoLoad);

            GraphBitmap = new Bitmap(BitmapEdgeLength, BitmapEdgeLength, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics tempgraphics = Graphics.FromImage(GraphBitmap);
            tempgraphics.DrawRectangle(Pens.Black, 0, 0, BitmapEdgeLength, BitmapEdgeLength);
            tempgraphics.Dispose();

            //add custom paint event handler to draw all nodes and edges
            Paint += new PaintEventHandler(DrawNodes);

            //parse story, and not get cancelled xD
            if (Context.ParseFile() || Context.GotCancelled)
            {
                NodeToHighlight = Context.GetNodes()[0];
                FillBitMap();
            }
            else
            {
                //else we quit
                Close();
            }
        }

        private void FillBitMap()
        {
            //draw to image
            Graphics graphics = Graphics.FromImage(GraphBitmap);
            //go on displaying graph
            foreach (Node node in Context.GetNodes())
            {
                //draw node
                graphics.FillEllipse(Brushes.RoyalBlue, node.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, node.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);

                //draw edges
                foreach (Node child in node.ChildNodes)
                {
                    //draw edge to child, default colour
                    graphics.DrawLine(
                        new Pen(Color.FromArgb(20, 20, 20), 2f),
                        (node.Position.X) + BitmapEdgeLength / 2,
                        (node.Position.Y) + BitmapEdgeLength / 2,
                        (child.Position.X) + BitmapEdgeLength / 2,
                        (child.Position.Y) + BitmapEdgeLength / 2);
                }
            }
            //allow paint handler to draw
            ReadyToDraw = true;
            Cursor = Cursors.Default;
            graphics.Dispose();
            Invalidate();
        }

        private void DrawHighlightNode()
        {
            //draw to image
            Graphics graphics = Graphics.FromImage(GraphBitmap);
            //draw parents and edges in colour
            foreach (Node node in NodeToHighlight.ParentNodes)
            {
                //draw line
                graphics.DrawLine(
                    new Pen(Color.Coral, 2f),
                    (node.Position.X) + BitmapEdgeLength / 2,
                    (node.Position.Y) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.X) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.Y) + BitmapEdgeLength / 2);
                //overdraw line with node
                graphics.FillEllipse(Brushes.OrangeRed, node.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, node.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);
            }

            //draw childs and edges in colour
            foreach (Node node in NodeToHighlight.ChildNodes)
            {
                //draw line
                graphics.DrawLine(
                    new Pen(Color.Coral, 2f),
                    (node.Position.X) + BitmapEdgeLength / 2,
                    (node.Position.Y) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.X) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.Y) + BitmapEdgeLength / 2);
                //draw node over line
                graphics.FillEllipse(Brushes.OrangeRed, node.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, node.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);
            }

            //redraw node itself
            graphics.FillEllipse(Brushes.DarkRed, NodeToHighlight.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, NodeToHighlight.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);
            graphics.Dispose();
            Invalidate();
        }

        private void DeleteHighlightDrawing()
        {
            //draw to image
            Graphics graphics = Graphics.FromImage(GraphBitmap);
            //draw parents and edges in colour
            foreach (Node node in NodeToHighlight.ParentNodes)
            {
                //draw line
                graphics.DrawLine(
                    new Pen(Color.FromArgb(20, 20, 20), 2f),
                    (node.Position.X) + BitmapEdgeLength / 2,
                    (node.Position.Y) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.X) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.Y) + BitmapEdgeLength / 2);
                //overdraw line with node
                graphics.FillEllipse(Brushes.RoyalBlue, node.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, node.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);
            }
            //draw childs and edges in colour
            foreach (Node node in NodeToHighlight.ChildNodes)
            {
                //draw line
                graphics.DrawLine(
                    new Pen(Color.FromArgb(20, 20, 20), 2f),
                    (node.Position.X) + BitmapEdgeLength / 2,
                    (node.Position.Y) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.X) + BitmapEdgeLength / 2,
                    (NodeToHighlight.Position.Y) + BitmapEdgeLength / 2);
                //draw node over line
                graphics.FillEllipse(Brushes.RoyalBlue, node.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, node.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);
            }

            //redraw node itself
            graphics.FillEllipse(Brushes.RoyalBlue, NodeToHighlight.Position.X - Nodesize / 2 + BitmapEdgeLength / 2, NodeToHighlight.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2, Nodesize, Nodesize);
            graphics.Dispose();
            Invalidate();
        }

        private void DrawNodes(object sender, PaintEventArgs e)
        {
            if (ReadyToDraw)
            {
                //convert image
                GraphToScreen(-BitmapEdgeLength / 2, -BitmapEdgeLength / 2, out float ImageScreenX, out float ImageScreenY);
                //display image with scaling applied
                e.Graphics.DrawImage(GraphBitmap, ImageScreenX, ImageScreenY, BitmapEdgeLength * Scaling, BitmapEdgeLength * Scaling);
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

        private void HandleKeyBoard(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                DeleteHighlightDrawing();
                selector++;
                NodeToHighlight = Context.GetNodes()[selector];
                DrawHighlightNode();
            }
            else if (e.KeyCode == Keys.D)
            {
                DeleteHighlightDrawing();
                if (selector > 0) selector--;
                NodeToHighlight = Context.GetNodes()[selector];
                DrawHighlightNode();
            }
        }

        private void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;
                case MouseButtons.None:
                    //end of pan
                    if (CurrentlyInPan)
                    {
                        CurrentlyInPan = false;
                    }
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    //start of pan
                    if (!CurrentlyInPan)
                    {
                        CurrentlyInPan = true;
                        //get current position in screen coordinates when we start to pan
                        StartPanOffsetX = e.X;
                        StartPanOffsetY = e.Y;
                    }
                    //in pan
                    else if (CurrentlyInPan)
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
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    //end of pan
                    if (CurrentlyInPan)
                    {
                        CurrentlyInPan = false;
                    }
                    break;
            }

            //everything else, scrolling for example
            if (e.Delta != 0)
            {
                //get last mouse position in world space before the zoom so we can
                //offset back by the distance in world space we got shifted by zooming
                ScreenToGraph(e.X, e.Y, out BeforeZoomMouseX, out BeforeZoomMouseY);
                //reset last
                //WHEEL_DELTA = 120, as per windows documentation
                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
                if (e.Delta > 0)
                {
                    Scaling *= 1.2f;
                }
                else if (e.Delta < 0)
                {
                    Scaling *= 0.8f;
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
