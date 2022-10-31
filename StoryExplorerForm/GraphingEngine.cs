using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    internal sealed class GraphingEngine
    {
        public const int BitmapEdgeLength = 7000;
        public const int HalfBitmapEdgeLength = BitmapEdgeLength / 2;
        public const int NegativeHalfBitmapEdgeLength = -HalfBitmapEdgeLength;
        public const int Nodesize = 16;

        public readonly ContextProvider Context;
        public bool ReadyToDraw = false;

        private readonly Color DefaultEdgeColor = Color.FromArgb(30, 30, 30);
        private readonly Color DefaultMaleColor = Color.Coral;
        private readonly Color DefaultColor = Color.DarkBlue;
        private readonly Color DefaultFemaleColor = Color.DarkTurquoise;
        private readonly StoryExplorer Explorer;
        private readonly Bitmap GraphBitmap;
        private readonly Graphics MainGraphics;
        private readonly Label NodeInfoLabel;

        private float AfterZoomMouseX = 0f;
        private float AfterZoomMouseY = 0f;
        private float BeforeZoomMouseX = 0f;
        private float BeforeZoomMouseY = 0f;

        private bool CurrentlyInPan = false;
        private Node highlightedNode = Node.NullNode;
        private Node infoNode = Node.NullNode;
        private bool IsShiftPressed = false;
        private bool IsCtrlPressed = false;
        private Node LastInfoNode = Node.NullNode;
        private Color LastNodeColor = Color.DarkBlue;

        private float OffsetX = -HalfBitmapEdgeLength;
        private float OffsetY = -HalfBitmapEdgeLength;
        private float Scaling = 0.3f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;

        private int bytesPerPixel;
        private int heightInPixels;
        private int widthInPixels;

        /*
        unsafe
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            byte* ptrFirstPixel = (byte*)bitmapData.Scan0;
         
            for (int y = 0; y < heightInPixels; y++)
            {
                byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    int oldBlue = currentLine[x];
                    int oldGreen = currentLine[x + 1];
                    int oldRed = currentLine[x + 2];
 
                    // calculate new pixel value
                    currentLine[x] = (byte)oldBlue;
                    currentLine[x + 1] = (byte)oldGreen;
                    currentLine[x + 2] = (byte)oldRed;
                }
            }
            processedBitmap.UnlockBits(bitmapData);
        }
        */

        public GraphingEngine(ContextProvider context, StoryExplorer explorer, Label nodeInfoLabel)
        {
            Context = context;
            Explorer = explorer;
            NodeInfoLabel = nodeInfoLabel;

            GraphBitmap = new Bitmap(BitmapEdgeLength, BitmapEdgeLength, PixelFormat.Format24bppRgb);

            bytesPerPixel = Image.GetPixelFormatSize(GraphBitmap.PixelFormat) / 8;
            heightInPixels = GraphBitmap.Height;
            widthInPixels = GraphBitmap.Width * bytesPerPixel;

            MainGraphics = Graphics.FromImage(GraphBitmap);
            MainGraphics.DrawRectangle(Pens.Black, 0, 0, BitmapEdgeLength, BitmapEdgeLength);

            ClickedNodeChanged += new ClickedNodeChangedHandler(HighlightClickedNodeHandler);
            ClickedNodeChanged += new ClickedNodeChangedHandler(DisplayNodeInfoHandler);
        }

        public delegate void ClickedNodeChangedHandler(object sender, ClickedNodeChangeArgs e);
        public event ClickedNodeChangedHandler ClickedNodeChanged;

        public Node HighlightedNode
        {
            get
            {
                return highlightedNode;
            }
            set
            {
                //clear highlight
                if (!IsShiftPressed) RemoveHighlight();
                if (value != null)
                {
                    //set new value
                    if (!IsShiftPressed) highlightedNode = value;
                    ClickedNodeChanged(this, new ClickedNodeChangeArgs(value, ClickedNodeTypes.Highlight));
                }
                else
                {
                    highlightedNode = Node.NullNode;
                }
            }
        }

        public Node InfoNode
        {
            get
            {
                return infoNode;
            }
            set
            {
                if (value != null)
                {
                    infoNode = value;
                    ClickedNodeChanged(this, new ClickedNodeChangeArgs(value, ClickedNodeTypes.Info));
                }
                else
                {
                    infoNode = Node.NullNode;
                }
            }
        }

        public void DrawNodesPaintHandler(object sender, PaintEventArgs e)
        {
            if (ReadyToDraw)
            {
                //convert image
                GraphToScreen(NegativeHalfBitmapEdgeLength, NegativeHalfBitmapEdgeLength, out float ImageScreenX, out float ImageScreenY);
                //display image with scaling applied
                e.Graphics.DrawImage(GraphBitmap, ImageScreenX, ImageScreenY, BitmapEdgeLength * Scaling, BitmapEdgeLength * Scaling);
            }
        }

        /// <summary>
        /// converts graph coordinates into the corresponding screen coordinates, taking into account all transformations/zoom
        /// </summary>
        /// <param name="graphX">x in the graphics coords</param>
        /// <param name="graphY">y in the graphics coords</param>
        /// <param name="screenX">x in the screens coords</param>
        /// <param name="screenY">y in the screens coords</param>
        public void GraphToScreen(float graphX, float graphY, out float screenX, out float screenY)
        {
            screenX = (graphX - OffsetX) * Scaling;
            screenY = (graphY - OffsetY) * Scaling;
        }

        public void HandleKeyBoard(object sender, KeyEventArgs e)
        {
            //get the shift key state so we can determine later if we want to redraw the tree on node selection or not
            IsShiftPressed = e.KeyData == (Keys.ShiftKey | Keys.Shift);
            //determine ctrl state for node moving
            IsCtrlPressed = e.KeyData == (Keys.ControlKey | Keys.Control);
        }

        public void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    SetNewHighlightNode(e.Location);
                    break;
                case MouseButtons.None:
                    EndPan();
                    break;
                case MouseButtons.Right:
                    SetNewInfoNode(e.Location);
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

        public void PaintAllNodes()
        {
            //go on displaying graph
            for (int i = 0; i < Context.Nodes.Count; i++)
            {
                //draw node
                DrawColouredNode(Context.Nodes[i], Context.Nodes[i].Gender == Gender.Female ? DefaultFemaleColor : Context.Nodes[i].Gender == Gender.Male ? DefaultMaleColor : DefaultColor);
                //draw edges to children, default colour
                for (int j = 0; j < Context.Nodes[i].ChildNodes.Count; j++)
                {
                    DrawEdge(Context.Nodes[i], Context.Nodes[i].ChildNodes[j], DefaultEdgeColor);
                }
            }
            //allow paint handler to draw
            ReadyToDraw = true;
        }

        /// <summary>
        /// converts screen coordinates into the corresponding graph coordinates, taking into account all transformations/zoom
        /// </summary>
        /// <param name="screenX">The x coord in screen coordinates</param>
        /// <param name="screenY">The y coord in screen coordinates</param>
        /// <param name="graphX">The returned x coord in graph coordinate space</param>
        /// <param name="graphY">The returned y coord in graph coordinate space</param>
        public void ScreenToGraph(float screenX, float screenY, out float graphX, out float graphY)
        {
            graphX = screenX / Scaling + OffsetX;
            graphY = screenY / Scaling + OffsetY;
        }

        private void DisplayNodeInfo(Node infoNode, bool colourNode)
        {
            //display info on new node
            if (infoNode != Node.NullNode)
            {
                NodeInfoLabel.Visible = true;
                //create header
                string header = $"{infoNode.Type} - {infoNode.ID}".ConstrainLength();
                if (infoNode.Gender != Gender.None) header = header + $" - {infoNode.Gender} only".ConstrainLength();

                //create info
                //strip text of all VA performance hints, embedded in []. if user wants it
                string info;
                if (Properties.Settings.Default.displayVAHints) { info = infoNode.Text.ConstrainLength(); }
                else { info = infoNode.Text.RemoveVAHints().ConstrainLength(); }

                //create seperator
                string seperator = "\n";
                for (int i = 0; i <= Math.Min(Utils.MaxTextLength, Math.Max(header.Length, info.Length)); i++)
                {
                    seperator += "#";
                }
                seperator += "\n";

                NodeInfoLabel.Text = header + seperator + info;

                if (colourNode)
                {
                    RemoveLastInfoNode(infoNode);
                    DrawColouredNode(infoNode, Color.ForestGreen);
                }
            }
            else //remove highlight display
            {
                NodeInfoLabel.Visible = false;
            }

            Explorer.Invalidate();
        }

        private void DisplayNodeInfoHandler(object sender, ClickedNodeChangeArgs e)
        {
            //display info on new node
            if (e.ClickType == ClickedNodeTypes.Info)
            {
                DisplayNodeInfo(e.ChangedNode, true);
            }
        }

        private void DrawColouredNode(Node node, Color color)
        {
            if (node.Type != NodeType.Event && node.Type != NodeType.Criterion)
            {
                MainGraphics.FillEllipse(
                               new SolidBrush(color),
                               node.Position.X - Nodesize / 2 + HalfBitmapEdgeLength,
                               node.Position.Y - Nodesize / 2 + HalfBitmapEdgeLength,
                               Nodesize,
                               Nodesize);
            }
        }

        private void DrawEdge(Node node1, Node node2, Color color)
        {
            if (node1.Type != NodeType.Event && node1.Type != NodeType.Criterion && node2.Type != NodeType.Event && node2.Type != NodeType.Criterion)
            {
                MainGraphics.DrawLine(
                new Pen(color, 3f),
                (node1.Position.X) + HalfBitmapEdgeLength,
                (node1.Position.Y) + HalfBitmapEdgeLength,
                (node2.Position.X) + HalfBitmapEdgeLength,
                (node2.Position.Y) + HalfBitmapEdgeLength);
            }
        }

        private void DrawEdges(List<Node> nodes, Color color)
        {
            Point[] points = new Point[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                points[i] = nodes[i].Position;
                points[i].X += HalfBitmapEdgeLength;
                points[i].Y += HalfBitmapEdgeLength;
            }
            MainGraphics.DrawLines(new Pen(color, 3f), points);
        }

        private void DrawHighlightNodeTree()
        {
            if (HighlightedNode != Node.NullNode)
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

        private void EndPan()
        {
            //end of pan
            if (CurrentlyInPan)
            {
                CurrentlyInPan = false;
            }
        }

        private void HighlightClickedNodeHandler(object sender, ClickedNodeChangeArgs e)
        {
            if (e.ClickType == ClickedNodeTypes.Highlight)
            {
                TranslationManager translationManager = TabManager.ActiveTranslationManager;
                //tell translationmanager to update us or not when selected
                translationManager.UpdateStoryExplorerSelection = !IsShiftPressed;
                //select line in translation manager
                translationManager.SelectLine(e.ChangedNode.ID);
                //put info up
                DisplayNodeInfo(e.ChangedNode, false);
                //draw nodes
                if (!IsShiftPressed) DrawHighlightNodeTree();
            }
        }

        private void RemoveHighlight()
        {
            if (HighlightedNode != null)
            {
                //draw over parents
                DrawNodeSet(
                    HighlightedNode.ParentNodes,
                    HighlightedNode,
                    0,
                    1,
                    DefaultMaleColor,
                    DefaultEdgeColor,
                    false);
                //draw over childs
                DrawNodeSet(
                    HighlightedNode.ChildNodes,
                    HighlightedNode,
                    0,
                    6,
                    HighlightedNode.Gender == Gender.Female ? DefaultFemaleColor : HighlightedNode.Gender == Gender.Male ? DefaultMaleColor : DefaultColor,
                    DefaultEdgeColor,
                    false);

                //redraw node itself
                DrawColouredNode(HighlightedNode, HighlightedNode.Gender == Gender.Female ? DefaultFemaleColor : HighlightedNode.Gender == Gender.Male ? DefaultMaleColor : DefaultColor);
                Explorer.Invalidate();

            }
        }

        private void RemoveLastInfoNode(Node infoNode)
        {
            //remove last node highlight
            if (LastInfoNode != Node.NullNode)
            {
                //remove old colour from node
                DrawColouredNode(LastInfoNode, LastNodeColor);
            }

            if (infoNode != Node.NullNode)
            {
                LastNodeColor = GraphBitmap.GetPixel(infoNode.Position.X + HalfBitmapEdgeLength, infoNode.Position.Y + HalfBitmapEdgeLength);
                if (LastNodeColor == DefaultEdgeColor)
                {//reset colour if it is gray, can happen due to drawing order
                    LastNodeColor = LastInfoNode.Gender == Gender.Female ? DefaultFemaleColor : LastInfoNode.Gender == Gender.Male ? DefaultMaleColor : DefaultColor;
                }
            }
        }

        private void SetNewHighlightNode(Point mouseLocation)
        {
            Node ClickedNode = UpdateClickedNode(mouseLocation);
            if (HighlightedNode != ClickedNode && ClickedNode != Node.NullNode)
            {
                HighlightedNode = ClickedNode;
            }
        }

        private void SetNewInfoNode(Point mouseLocation)
        {
            //get new clicked node
            if (InfoNode != Node.NullNode) LastInfoNode = InfoNode;

            InfoNode = UpdateClickedNode(mouseLocation);
        }

        private void SetPanOffset(Point location)
        {

            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private Node UpdateClickedNode(Point mouseLocation)
        {
            //handle position input
            ScreenToGraph(mouseLocation.X - Nodesize, mouseLocation.Y - Nodesize, out float mouseLeftX, out float mouseUpperY);
            ScreenToGraph(mouseLocation.X + Nodesize, mouseLocation.Y + Nodesize, out float mouseRightX, out float mouseLowerY);

            foreach (Node node in Context.Nodes)
            {
                if (mouseLowerY > node.Position.Y && mouseUpperY < node.Position.Y)
                {
                    if (mouseRightX > node.Position.X && mouseLeftX < node.Position.X)
                    {
                        if (node.Type != NodeType.Event && node.Type != NodeType.Criterion)
                        {
                            return node;
                        }
                    }
                }
            }

            return Node.NullNode;
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

        private void UpdatePanOffset(Point location)
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

        private void UpdateScaling(MouseEventArgs e)
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
    }
}
