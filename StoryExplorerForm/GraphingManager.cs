using HousePartyTranslator.Helpers;
using HousePartyTranslator.StoryExplorerForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    class GraphingManager
    {
        private bool CurrentlyInPan = false;
        private const int MaxTextLength = 100;
        private float AfterZoomMouseX = 0f;
        private float AfterZoomMouseY = 0f;
        private float BeforeZoomMouseX = 0f;
        private float BeforeZoomMouseY = 0f;
        private float OffsetX = 0f;
        private float OffsetY = 0f;
        private float Scaling = 0.3f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private Color LastNodeColor = Color.DarkBlue;
        private Node InfoNode;
        private Node LastInfoNode;
        private Node HighlightedNode;
        private readonly Bitmap GraphBitmap;
        private readonly Color DefaultEdgeColor = Color.FromArgb(30, 30, 30);
        private readonly Color DefaultNodeColor = Color.DarkBlue;
        private readonly ContextProvider Context;
        private readonly StoryExplorer Explorer;
        private readonly Label NodeInfoLabel;
        public bool ReadyToDraw = false;
        public const int BitmapEdgeLength = 7000;
        public const int Nodesize = 16;
        public readonly Graphics MainGraphics;

        public GraphingManager(ContextProvider context, StoryExplorer explorer, Label nodeInfoLabel)
        {
            Context = context;
            Explorer = explorer;
            NodeInfoLabel = nodeInfoLabel;

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

        public void RemoveHighlight()
        {
            if (HighlightedNode != null)
            {
                //draw over parents
                DrawNodeSet(
                    HighlightedNode.ParentNodes,
                    HighlightedNode,
                    0,
                    1,
                    DefaultNodeColor,
                    DefaultEdgeColor,
                    false);
                //draw over childs
                DrawNodeSet(
                    HighlightedNode.ChildNodes,
                    HighlightedNode,
                    0,
                    6,
                    DefaultNodeColor,
                    DefaultEdgeColor,
                    false);

                //redraw node itself
                DrawColouredNode(HighlightedNode, DefaultNodeColor);
                Explorer.Invalidate();

            }
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
            if (HighlightedNode != null)
            {
                //draw parents first (one layer only)
                DrawNodeSet(
                    HighlightedNode.ParentNodes,
                    HighlightedNode,
                    0,
                    1,
                    Color.White,
                    Color.DarkOrchid,
                    false);
                //then childs
                DrawNodeSet(
                    HighlightedNode.ChildNodes,
                    HighlightedNode,
                    0,
                    6,
                    Color.Red,
                    Color.DeepPink,
                    true);

                //then redraw node itself
                DrawColouredNode(HighlightedNode, Color.DarkRed);
                Explorer.Invalidate();
            }
        }

        private void DrawNodeSet(List<Node> nodes, Node previousNode, int depth, int maxDepth, Color nodeColor, Color edgeColor, bool diluteColor)
        {
            if (depth++ < maxDepth)
            {
                //highlight childs
                foreach (Node node in nodes)
                {
                    if (diluteColor)
                    {
                        DrawNodeSet(node.ChildNodes, node, depth, maxDepth, Color.FromArgb((int)(nodeColor.ToArgb() / 1.3d)), Color.FromArgb((int)(edgeColor.ToArgb() / 1.3d)), true);
                    }
                    else
                    {
                        DrawNodeSet(node.ChildNodes, node, depth, maxDepth, nodeColor, edgeColor, false);
                    }

                    //draw line
                    DrawEdge(node, previousNode, edgeColor);
                    //draw node over line
                    DrawColouredNode(node, nodeColor);
                }
            }
        }

        public void PaintAllNodes()
        {
            //go on displaying graph
            foreach (Node node in Context.GetNodes())
            {
                //draw node
                DrawColouredNode(node, DefaultNodeColor);
                //draw edges
                foreach (Node child in node.ChildNodes)
                {
                    //draw edge to child, default colour
                    DrawEdge(node, child, DefaultEdgeColor);
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

        public void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    DisplayNodeInfo(e.Location);
                    break;
                case MouseButtons.None:
                    EndPan();
                    break;
                case MouseButtons.Right:
                    HighlightClickedNode(e.Location);
                    break;
                case MouseButtons.Middle:
                    UpdatePan(e.Location);
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    EndPan();
                    break;
            }
            //everything else, scrolling for example
            if (e.Delta != 0)
            {
                UpdateScaling(e);
                //redraw
                Explorer.Invalidate();
            }
        }

        private void DisplayNodeInfo(Point mouseLocation)
        {
            //get new clicked node
            if (InfoNode != null) LastInfoNode = InfoNode;
            InfoNode = UpdateClickedNode(mouseLocation);
            //display info on new node
            if (InfoNode != null)
            {
                NodeInfoLabel.Visible = true;
                //create header
                string header = ConstrainLength($"{InfoNode.Type} - {InfoNode.ID}");
                //strip text of all VA performance hints, embedded in []

                //create info
                string info = ConstrainLength(RemoveVAHints(InfoNode.Text));
                //create seperator
                string seperator = "\n";
                for (int i = 0; i <= Math.Min(MaxTextLength, Math.Max(header.Length, info.Length)); i++)
                {
                    seperator += "#";
                }
                seperator += "\n";

                NodeInfoLabel.Text = header + seperator + info;

                RemoveLastInfoNode(InfoNode);

                DrawColouredNode(InfoNode, Color.ForestGreen);
            }
            else //remove highlight display
            {
                NodeInfoLabel.Visible = false;
            }

            Explorer.Invalidate();
        }

        private string RemoveVAHints(string input)
        {
            bool inVAHint = false;
            string output = "";
            foreach (char character in input)
            {
                if (character == '[' && !inVAHint)
                {
                    inVAHint = true;
                }
                else if (character == ']' && inVAHint)
                {
                    inVAHint = false;
                }
                else if (!inVAHint)
                {
                    output += character;
                }
            }

            return output;
        }

        private string ConstrainLength(string input)
        {
            string output = "";
            int inputLength = input.Length;

            for (int i = 0; i <= (inputLength / MaxTextLength); i++)
            {
                int possibleEnd = Math.Min(MaxTextLength, input.Length);
                output += input.Substring(0, possibleEnd);
                if (possibleEnd == MaxTextLength) output += " \n";
                input = input.Remove(0, possibleEnd);
            }

            return output;
        }

        private void HighlightClickedNode(Point mouseLocation)
        {
            Node ClickedNode = UpdateClickedNode(mouseLocation);
            if (HighlightedNode != ClickedNode && ClickedNode != null)
            {
                RemoveHighlight();
                HighlightedNode = ClickedNode;
            }
            DrawHighlightNodeTree();
        }

        private void RemoveLastInfoNode(Node infoNode)
        {
            //remove last node highlight
            if (LastInfoNode != null)
            {
                //remove old colour from node
                DrawColouredNode(LastInfoNode, LastNodeColor);
            }

            if (infoNode != null)
            {
                LastNodeColor = GraphBitmap.GetPixel(infoNode.Position.X + BitmapEdgeLength / 2, infoNode.Position.Y + BitmapEdgeLength / 2);
                if (LastNodeColor == DefaultEdgeColor)
                {//reset colour if it is gray, can happen due to drawing order
                    LastNodeColor = DefaultNodeColor;
                }
            }
        }

        private Node UpdateClickedNode(Point mouseLocation)
        {
            //handle position input
            ScreenToGraph(mouseLocation.X - Nodesize, mouseLocation.Y - Nodesize, out float mouseLeftX, out float mouseUpperY);
            ScreenToGraph(mouseLocation.X + Nodesize, mouseLocation.Y + Nodesize, out float mouseRightX, out float mouseLowerY);

            foreach (Node node in Context.GetNodes())
            {
                if (mouseLowerY > node.Position.Y && mouseUpperY < node.Position.Y)
                {
                    if (mouseRightX > node.Position.X && mouseLeftX < node.Position.X)
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        private void EndPan()
        {
            //end of pan
            if (CurrentlyInPan)
            {
                CurrentlyInPan = false;
            }
        }

        private void UpdatePan(Point mouseLocation)
        {
            //start of pan
            if (!CurrentlyInPan)
            {
                CurrentlyInPan = true;
                //get current position in screen coordinates when we start to pan
                SetPanOffset(mouseLocation);
            }
            //in pan
            else if (CurrentlyInPan)
            {
                UpdatePanOffset(mouseLocation);
                //redraw
                Explorer.Invalidate();
            }
        }

        public void HandleKeyBoard(object sender, KeyEventArgs e)
        {

        }
    }
}
