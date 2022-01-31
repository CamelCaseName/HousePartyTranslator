using HousePartyTranslator.Helpers;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    class GraphingManager
    {
        public readonly Graphics MainGraphics;
        private float AfterZoomMouseX = 0f;
        private float AfterZoomMouseY = 0f;
        private float BeforeZoomMouseX = 0f;
        private float BeforeZoomMouseY = 0f;
        private float OffsetX = 0f;
        private float OffsetY = 0f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        public bool ReadyToDraw = false;
        public const int BitmapEdgeLength = 6000;
        public const int Nodesize = 16;
        public float Scaling = 0.3f;
        public Node NodeToHighlight;
        public readonly Bitmap GraphBitmap;
        private readonly ContextProvider Context;
        private readonly StoryExplorerForm.StoryExplorer Explorer;

        public GraphingManager(ContextProvider context, StoryExplorerForm.StoryExplorer explorer)
        {
            Context = context;
            Explorer = explorer;

            GraphBitmap = new Bitmap(BitmapEdgeLength, BitmapEdgeLength, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            MainGraphics = Graphics.FromImage(GraphBitmap);
            MainGraphics.DrawRectangle(Pens.Black, 0, 0, BitmapEdgeLength, BitmapEdgeLength);

            OffsetX = -BitmapEdgeLength / 2;
            OffsetY = -BitmapEdgeLength / 2;
        }

        public void DrawNodesPaintHandler(object sender, PaintEventArgs e)
        {
            if (ReadyToDraw)
            {
                //convert image
                GraphToScreen(-BitmapEdgeLength / 2, -BitmapEdgeLength / 2, out float ImageScreenX, out float ImageScreenY);
                //display image with scaling applied
                e.Graphics.DrawImage(GraphBitmap, ImageScreenX, ImageScreenY, BitmapEdgeLength * Scaling, BitmapEdgeLength * Scaling);
            }
        }

        public void DrawOverHighlight()
        {
            //draw parents and edges in colour
            foreach (Node node in Enumerable.Concat(NodeToHighlight.ParentNodes, NodeToHighlight.ChildNodes))
            {
                //draw line
                DrawEdge(node, NodeToHighlight, Color.FromArgb(30, 30, 30));
                //draw node over line
                DrawColouredNode(node, Color.DarkBlue);

                //highlight second childs
                foreach (Node node2 in Enumerable.Concat(node.ParentNodes, node.ChildNodes))
                {
                    //draw line
                    DrawEdge(node2, node, Color.FromArgb(30, 30, 30));
                    //draw node over line
                    DrawColouredNode(node2, Color.DarkBlue);
                }
            }

            //redraw node itself
            DrawColouredNode(NodeToHighlight, Color.DarkBlue);
            Explorer.Invalidate();
        }

        public void SetPanOffset(Point location)
        {

            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        public void UpdatePanOffset(Point location)
        {
            //better scaling thanks to the one lone coder
            //https://www.youtube.com/watch?v=ZQ8qtAizis4
            //update opffset by the difference in screen coordinates we travelled so far
            OffsetX -= (location.X - StartPanOffsetX) / Scaling;
            OffsetY -= (location.Y - StartPanOffsetY) / Scaling;

            //replace old start by new start coordinates so we only look at one interval,
            //and do not accumulate the change in position
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        public void DrawColouredNode(Node node, Color color)
        {
            MainGraphics.FillEllipse(
                new SolidBrush(color),
                node.Position.X - Nodesize / 2 + BitmapEdgeLength / 2,
                node.Position.Y - Nodesize / 2 + BitmapEdgeLength / 2,
                Nodesize,
                Nodesize);
        }

        public void DrawEdge(Node node1, Node node2, Color color)
        {
            MainGraphics.DrawLine(
                new Pen(color, 3f),
                (node1.Position.X) + BitmapEdgeLength / 2,
                (node1.Position.Y) + BitmapEdgeLength / 2,
                (node2.Position.X) + BitmapEdgeLength / 2,
                (node2.Position.Y) + BitmapEdgeLength / 2);
        }

        public void DrawHighlightNodeTree()
        {
            //draw parents and edges in colour
            foreach (Node node in Enumerable.Concat(NodeToHighlight.ParentNodes, NodeToHighlight.ChildNodes))
            {
                //draw line
                DrawEdge(node, NodeToHighlight, Color.FromArgb(255, 100, 0));
                //draw node over line
                DrawColouredNode(node, Color.Red);

                //highlight second childs
                foreach (Node node2 in node.ChildNodes)
                {
                    //draw line
                    DrawEdge(node2, node, Color.FromArgb(125, 50, 0));
                    //draw node over line
                    DrawColouredNode(node2, Color.DarkOrange);
                }
            }

            //redraw node itself
            DrawColouredNode(NodeToHighlight, Color.DarkRed);
            Explorer.Invalidate();
        }


        public void PaintAllNodes()
        {
            //go on displaying graph
            foreach (Node node in Context.GetNodes())
            {
                //draw node
                DrawColouredNode(node, Color.DarkBlue);
                //draw edges
                foreach (Node child in node.ChildNodes)
                {
                    //draw edge to child, default colour
                    DrawEdge(node, child, Color.FromArgb(30, 30, 30));
                }
            }
            //allow paint handler to draw
            ReadyToDraw = true;
            Explorer.Cursor = Cursors.Default;
            Explorer.Invalidate();
        }

        public void UpdateScaling(MouseEventArgs e)
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
        }

        //converts graph coordinates into the corresponding screen coordinates, taking into account all transformations/zoom
        public void GraphToScreen(float graphX, float graphY, out float screenX, out float screenY)
        {
            screenX = (graphX - OffsetX) * Scaling;
            screenY = (graphY - OffsetY) * Scaling;
        }

        //converts screen coordinates into the corresponding graph coordinates, taking into account all transformations/zoom
        public void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling + OffsetX;
            graphY = screenY / Scaling + OffsetY;
        }
    }
}
