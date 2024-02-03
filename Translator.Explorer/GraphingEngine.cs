using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Core;
using Translator.Core.Helpers;
using Translator.Explorer.Graph;
using Translator.Explorer.Window;
using static Translator.Explorer.JSONItems.StoryEnums;
using Settings = Translator.Desktop.InterfaceImpls.WinSettings;

namespace Translator.Explorer
{
    [SupportedOSPlatform("Windows")]
    public sealed class GraphingEngine
    {
        public const float ColorFactor = 0.7f;
        public bool DrewNodes = false;
        public bool publicNodesVisible = true;
        public DateTime StartTime = DateTime.MinValue;
        public DateTime FrameStartTime = DateTime.MinValue;
        public DateTime FrameEndTime = DateTime.MinValue;
        public int FrameCount { get; private set; }
        public static bool ShowExtendedInfo { get => Settings.ShowExtendedExplorerInfo; set => Settings.ShowExtendedExplorerInfo = value; }

        public readonly NodeProvider Provider;

        private readonly SolidBrush ColorBrush;
        private readonly Pen ColorPen;

        private static float Xmax = 0;
        private static float Ymax = 0;
        private static float Xmin = 0;
        private static float Ymin = 0;
        private static float MinEdgeLength = 0;

        private readonly StoryExplorer Explorer;
        private readonly Label NodeInfoLabel;

        private readonly Dictionary<Type, GroupBox> ExtendedInfoComponents = new();

        private float OffsetX;
        private float OffsetY;

        private bool CurrentlyInPan = false;
        private Node highlightedNode = Node.NullNode;
        private Node infoNode = Node.NullNode;
        private Node movingNode = Node.NullNode;
        private bool IsShiftPressed = false;
        private bool IsCtrlPressed = false;
        private bool MovingANode = false;
        private HashSet<Node> NodesHighlighted = new();
        private Cursor priorCursor = Cursors.Default;
        private readonly AdjustableArrowCap defaultArrowCap = new(StoryExplorerConstants.Nodesize / 3, StoryExplorerConstants.Nodesize / 4);
        private readonly AdjustableArrowCap smallArrowCap = new(StoryExplorerConstants.Nodesize / 6, StoryExplorerConstants.Nodesize / 8);

        private float Scaling = 0.3f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private float OldMouseMovingPosX;
        private float OldMouseMovingPosY;
        private float AfterZoomMouseX;
        private float AfterZoomMouseY;
        private float BeforeZoomMouseX;
        private float BeforeZoomMouseY;

        public GraphingEngine(NodeProvider provider, StoryExplorer explorer, Label nodeInfoLabel)
        {
            Provider = provider;
            Explorer = explorer;
            NodeInfoLabel = nodeInfoLabel;
            NodesHighlighted = new(Provider.Nodes.Count);

            OffsetX = (float)Explorer.ClientRectangle.X / 2;
            OffsetY = (float)Explorer.ClientRectangle.Y / 2;


            ColorBrush = new SolidBrush(Settings.DefaultNodeColor);
            ColorPen = new Pen(Settings.DefaultEdgeColor, 2f)
            {
                CustomEndCap = defaultArrowCap,
                StartCap = LineCap.Round
            };

            //dont run if we are in standalone
            if (Utils.IsInitialized)
            {
                ClickedNodeChanged += new ClickedNodeChangedHandler(HighlightClickedNodeHandler);
                ClickedNodeChanged += new ClickedNodeChangedHandler(DisplayNodeInfoHandler);
            }
            else
            {
                ClickedNodeChanged += (_, _) => { };
            }
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
                if (value is not null)
                {
                    //set new value
                    highlightedNode = value;
                    if (Provider.Nodes.Count != NodesHighlighted.Count) NodesHighlighted = new(Provider.Nodes.Count);
                    Explorer.SetNextButtonStates(value.ParentNodes.Count > 0, value.ChildNodes.Count > 0);
                    Center();
                    ClickedNodeChanged(this, new ClickedNodeChangeArgs(value, ClickedNodeTypes.Highlight));
                }
                else
                {
                    highlightedNode = Node.NullNode;
                    Explorer.SetNextButtonStates(false, false);
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
                if (value is not null)
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

        //increase drawing speed by switching to direct2d? either custom binding as needed or sharpdx?
        public void DrawNodesPaintHandler(object? sender, PaintEventArgs? e)
        {
            if (e is null) return;
            Graphics g = e.Graphics;

            FrameStartTime = DateTime.UtcNow;
            ++FrameCount;
            //set up values for this paint cycle
            MinEdgeLength = MathF.Pow(15 / Scaling, 2); // that one works

            //disables and reduces unused features
            g.ToLowQuality();

            //update canvas transforms
            g.TranslateTransform(-OffsetX * Scaling, -OffsetY * Scaling);
            g.ScaleTransform(Scaling, Scaling);
            Xmin = OffsetX - StoryExplorerConstants.Nodesize;
            Ymin = OffsetY - StoryExplorerConstants.Nodesize;
            Xmax = g.VisibleClipBounds.Right + StoryExplorerConstants.Nodesize;
            Ymax = g.VisibleClipBounds.Bottom + StoryExplorerConstants.Nodesize;

            if (Scaling < 0.1)
                ColorPen.CustomEndCap = smallArrowCap;
            else
                ColorPen.CustomEndCap = defaultArrowCap;

            PaintAllNodes(g);

            //overlay info and highlight
            DrawHighlightNodeTree(g);
            DrawMovingNodeMarker(g);
            DrawInfoNode(g);
            NodeInfoLabel.Refresh();
            FrameEndTime = DateTime.UtcNow;
#if DEBUG
            if ((FrameEndTime - FrameStartTime).TotalMilliseconds > 33)
                LogManager.Log("draw time too long for 30fps! Expected < 33, actual: " + (FrameEndTime - FrameStartTime).TotalMilliseconds);
#endif
        }

        public void Center()
        {
            if (!Settings.CenterNodeOnClick)
            {
                if (highlightedNode != Node.NullNode)
                    CenterOnNode(highlightedNode);
                else if (infoNode != Node.NullNode)
                    CenterOnNode(infoNode);
            }
        }

        public void CenterOnNode(Node node)
        {
            //todo implement corectly
            OffsetX = node.Position.X - (Xmax - OffsetX) / 2;
            OffsetY = node.Position.Y - (Ymax - OffsetY) / 2;
        }

        private void DrawMovingNodeMarker(Graphics g)
        {
            if (movingNode != Node.NullNode)
                DrawColouredNode(g, movingNode, Settings.MovingNodeColor, 1.2f);
        }

        public void HandleKeyBoard(object sender, KeyEventArgs e)
        {
            //get the shift key state so we can determine later if we want to redraw the tree on node selection or not
            IsShiftPressed = e.KeyData == (Keys.ShiftKey | Keys.Shift);
            IsCtrlPressed = e.KeyData == (Keys.Control | Keys.ControlKey);
        }

        public void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            //set old position for next frame/call
            switch (e.Button)
            {
                case MouseButtons.Left:
                    HandleNodeMovement(e.Location);
                    if (!IsCtrlPressed) HighlightedNode = GetClickedNode(e.Location, out _);
                    break;
                case MouseButtons.None:
                    EndPan();
                    break;
                case MouseButtons.Right:
                    InfoNode = GetClickedNode(e.Location, out _);
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
            //also end movement if we dont click but let go of ctrl
            if (!IsCtrlPressed && MovingANode) EndNodeMovement();
            //save mouse pos for next frame
            ScreenToGraph(e.Location.X, e.Location.Y, out OldMouseMovingPosX, out OldMouseMovingPosY);
        }

        private void HandleNodeMovement(Point MouseLocation)
        {
            if (!MovingANode && IsCtrlPressed)
                BeginMovingNodeMovement(MouseLocation);
            if (IsCtrlPressed && MovingANode && movingNode != Node.NullNode)
                UpdateMovingNodeMovement(MouseLocation);
            else
            {
                EndNodeMovement();
            }
        }

        private void BeginMovingNodeMovement(Point MouseLocation)
        {
            Node node = GetClickedNode(MouseLocation, out int index);
            //set new node if it is new, reset lock if applicable
            if (movingNode != node) movingNode.IsPositionLocked = false;
            if (priorCursor != Cursors.SizeAll) priorCursor = Explorer.Cursor;
            movingNode = node;
            movingNode.IsPositionLocked = true;
            MovingANode = true;

            Provider.SignalPositionChangeBegin(index);
        }

        private void UpdateMovingNodeMovement(Point MouseLocation)
        {
            //convert mouse position and adjust node position by mouse location delta
            ScreenToGraph(MouseLocation.X, MouseLocation.Y, out float MouseGraphX, out float MouseGraphY);
            movingNode.Position.X -= OldMouseMovingPosX - MouseGraphX;
            movingNode.Position.Y -= OldMouseMovingPosY - MouseGraphY;
            Explorer.Cursor = Cursors.SizeAll;
            //redraw
            Explorer.Invalidate();

            Provider.UpdatePositionChange(movingNode.Position.X, movingNode.Position.Y);
        }

        private void EndNodeMovement()
        {
            Explorer.Cursor = priorCursor;
            movingNode.IsPositionLocked = false;
            movingNode = Node.NullNode;
            MovingANode = false;
            Provider.EndPositionChange();
        }

        public void PaintAllNodes(Graphics g)
        {
            if (!Provider.Frozen) return;
            DrewNodes = false;
            Node node;
            //go on displaying graph
            for (int i = 0; i < Provider.Nodes.Count; i++)
            {
                node = Provider.Nodes[i];
                if (node.Position.X <= Xmax && node.Position.Y <= Ymax && node.Position.X >= Xmin && node.Position.Y >= Ymin)
                    DrawColouredNode(g, node);
            }
            int maxEdges = Math.Min(Provider.Nodes.Edges.Count, Settings.MaxEdgeCount);
            for (int i = 0; i < maxEdges; i++)
            {
                DrawEdge(g, Provider.Nodes.Edges[i].This, Provider.Nodes.Edges[i].Child);
            }
            DrewNodes = true;
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

        private void DisplayNodeInfo(Node node)
        {
            //display info on new node
            if (node != Node.NullNode)
            {
                NodeInfoLabel.Visible = true;
                //create header
                string header = $"{node.FileName}: {node.Type} - {node.ID}".ConstrainLength();
                if (node.Gender != Gender.None) header += $" - {node.Gender} only".ConstrainLength();

                //create info
                //strip text of all VA performance hints, embedded in []. if user wants it
                string info = Settings.Default.DisplayVoiceActorHints ? node.Text.ConstrainLength() : node.Text.RemoveVAHints().ConstrainLength();

                //create seperator
                string seperator = "\n";
                for (int i = 0; i <= Math.Min(Utils.MaxTextLength, Math.Max(header.Length, info.Length)); i++)
                {
                    seperator += "#";
                }
                seperator += "\n";

                NodeInfoLabel.Text = header + seperator + info;
                NodeInfoLabel.BringToFront();

                if (ShowExtendedInfo) DisplayExtendedNodeInfo(NodeInfoLabel.ClientRectangle, node);
            }
            else //remove highlight display
            {
                foreach (GroupBox box in ExtendedInfoComponents.Values)
                {
                    box.Visible = false;
                }
                NodeInfoLabel.Visible = false;
            }

            Explorer.Invalidate();
        }

        private void DisplayExtendedNodeInfo(Rectangle infoLabelRect, Node node)
        {
            foreach (GroupBox oldBox in ExtendedInfoComponents.Values)
            {
                oldBox.Visible = false;
            }
            if (node.Data is null) return;

            //use components if we have them already
            if (!ExtendedInfoComponents.TryGetValue(node.DataType, out GroupBox? box))
            {
                //create new and cache, then display
                box = ExtendedInfoUIBuilder.GetDisplayComponentsForType(
                    node.Data,
                    node.DataType,
                    new Size(Math.Clamp(NodeInfoLabel.ClientRectangle.Width, 340, 900), Explorer.ClientRectangle.Height - Explorer.Grapher.NodeInfoLabel.Height - 10));
                Explorer.Controls.Add(box);
                ExtendedInfoComponents.Add(node.DataType, box);
            }
            else
            {
                ExtendedInfoUIBuilder.FillDisplayComponents(box, node.Data);
            }
            box.Location = new Point(infoLabelRect.Left, infoLabelRect.Bottom + 5);
            box.MaximumSize = new Size(Math.Clamp(infoLabelRect.Width, 340, 900), Explorer.ClientRectangle.Height - Explorer.Grapher.NodeInfoLabel.Height - 10);
            box.Size = new Size(Math.Clamp(infoLabelRect.Width, 340, 900), 10);
            ExtendedInfoUIBuilder.SetEditableStates(box);
            box.BringToFront();
            box.Visible = true;
        }

        private void DisplayNodeInfoHandler(object sender, ClickedNodeChangeArgs e)
        {
            //display info on new node
            if (e.ClickType == ClickedNodeTypes.Info)
            {
                infoNode = e.ChangedNode;
                Explorer.Invoke(() => DisplayNodeInfo(e.ChangedNode));
            }
        }

        private void DrawColouredNode(Graphics g, Node node)
        {
            DrawColouredNode(g, node, ColorFromNode(node));
        }

        private void DrawColouredNode(Graphics g, Node node, Color color, float scale = 1.0f)
        {
            //dont draw node if it is too far away
            if (publicNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
            {
                ColorBrush.Color = color;
                g.FillEllipse(
                    ColorBrush,
                    node.Position.X - StoryExplorerConstants.Nodesize / 2 * scale,
                    node.Position.Y - StoryExplorerConstants.Nodesize / 2 * scale,
                    StoryExplorerConstants.Nodesize * scale,
                    StoryExplorerConstants.Nodesize * scale
                    );
            }
        }

        public void DrawEdge(Graphics g, Node node1, Node node2)
        {
            DrawEdge(g, node1, node2, Settings.DefaultEdgeColor);
        }

        public void DrawEdge(Graphics g, Node node1, Node node2, Color color, float width = 1.5f)
        {
            if (publicNodesVisible || node1.Type != NodeType.Event && node1.Type != NodeType.Criterion && node2.Type != NodeType.Event && node2.Type != NodeType.Criterion)
            {
                float x1 = node1.Position.X;
                float y1 = node1.Position.Y;
                float x2 = node2.Position.X;
                float y2 = node2.Position.Y;
                //sort out lines that would be too small on screen and ones where none of the ends are visible
                if (MathF.Pow(x1 - x2, 2) + MathF.Pow(y1 - y2, 2) > MinEdgeLength &&
                    (x1 < Xmax && x1 > Xmin && y1 < Ymax && y1 > Ymin ||
                    x2 < Xmax && x2 > Xmin && y2 < Ymax && y2 > Ymin))
                {
                    //any is visible
                    ColorPen.Color = color;
                    ColorPen.Width = width;
                    //todo adjust edge offsets to only touch the nodes in future (if only for small graphs so we dont compromise performance)?
                    g.DrawLine(ColorPen, x1, y1, x2, y2);
                }
                //none are visible, why draw?
            }
        }

        private void DrawHighlightNodeTree(Graphics g)
        {
            if (HighlightedNode != Node.NullNode)
            {
                NodesHighlighted.Clear();
                //then redraw node itself
                DrawColouredNode(g, HighlightedNode, Color.White, 1.7f);
                //then childs
                DrawHighlightNodeSet(
                    g,
                    HighlightedNode,
                    0,
                    StoryExplorerConstants.ColoringDepth,
                    Rainbow(0),
                    RainbowEdge(0));
            }
        }

        public static Color Rainbow(float progress)
        {
            float div = Math.Abs(progress % 1) * 6;
            int ascending = (int)(div % 1 * 255);
            int descending = 255 - ascending;

            return (int)div switch
            {
                0 => Color.FromArgb(255, 255, ascending, 127),
                1 => Color.FromArgb(255, descending, 255, 127),
                2 => Color.FromArgb(255, 127, 255, ascending),
                3 => Color.FromArgb(255, 127, descending, 255),
                4 => Color.FromArgb(255, ascending, 127, 255),
                // case 5:
                _ => Color.FromArgb(255, 255, 127, descending),
            };
        }

        public static Color RainbowEdge(float progress)
        {
            float div = Math.Abs(progress % 1) * 6;
            int ascending = (int)(div % 1 * 255);
            int descending = 255 - ascending;

            return (int)div switch
            {
                0 => Color.FromArgb(255, 255, ascending, 0),
                1 => Color.FromArgb(255, descending, 255, 0),
                2 => Color.FromArgb(255, 0, 255, ascending),
                3 => Color.FromArgb(255, 0, descending, 255),
                4 => Color.FromArgb(255, ascending, 0, 255),
                // case 5:
                _ => Color.FromArgb(255, 255, 0, descending),
            };
        }

        private void DrawInfoNode(Graphics g)
        {
            if (InfoNode != Node.NullNode)
                DrawColouredNode(g, InfoNode, Settings.InfoNodeColor);
        }

        private void DrawHighlightNodeSet(Graphics g, Node node, int depth, int maxDepth, Color nodeColor, Color edgeColor)
        {
            _ = NodesHighlighted.Add(node);

            //draw node 
            if (depth != 0)
                if (Settings.UseRainbowNodeColors)
                    DrawColouredNode(g, node, nodeColor);
                else
                    DrawColouredNode(g, node, ColorFromNode(node));

            if (depth++ < maxDepth)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (!NodesHighlighted.Contains(node.ChildNodes[i]))
                        DrawHighlightNodeSet(g, node.ChildNodes[i], depth, maxDepth, Rainbow((float)depth / 10), RainbowEdge((float)depth / 14));
                    if (Settings.UseRainbowEdgeColors)
                        DrawEdge(g, node, node.ChildNodes[i], edgeColor);
                    else
                        DrawEdge(g, node, node.ChildNodes[i], Color.LightGray, 2f);
                }
                for (int i = 0; i < node.ParentNodes.Count; i++)
                {
                    if (!NodesHighlighted.Contains(node.ParentNodes[i]))
                        DrawHighlightNodeSet(g, node.ParentNodes[i], depth, maxDepth, Rainbow((float)depth / 10), RainbowEdge((float)depth / 14));
                    if (Settings.UseRainbowEdgeColors)
                        DrawEdge(g, node.ParentNodes[i], node, edgeColor);
                    else
                        DrawEdge(g, node.ParentNodes[i], node, Color.LightGray, 2f);
                }
            }
        }

        private static Color ColorFromNode(Node node)
        {
            return node.Type switch
            {
                NodeType.Null => Settings.DefaultNodeColor,
                NodeType.Item => Settings.ItemNodeColor,
                NodeType.ItemGroup => Settings.ItemGroupNodeColor,
                NodeType.ItemAction => Settings.ActionNodeColor,
                NodeType.Event => Settings.EventNodeColor,
                NodeType.Criterion => Settings.CriterionNodeColor,
                NodeType.Response => Settings.ResponseNodeColor,
                NodeType.Dialogue => node.Gender switch
                {
                    Gender.Female => Settings.DialogueFemaleOnlyNodeColor,
                    Gender.Male => Settings.DialogueMaleOnlyNodeColor,
                    _ => Settings.DialogueNodeColor
                },
                NodeType.Quest => Settings.QuestNodeColor,
                NodeType.Achievement => Settings.AchievementNodeColor,
                NodeType.EventTrigger => Settings.ReactionNodeColor,
                NodeType.BGC => Settings.BGCNodeColor,
                NodeType.Value => Settings.ValueNodeColor,
                NodeType.Door => Settings.DoorNodeColor,
                NodeType.Inventory => Settings.InventoryNodeColor,
                NodeType.State => Settings.StateNodeColor,
                NodeType.Personality => Settings.PersonalityNodeColor,
                NodeType.Cutscene => Settings.CutsceneNodeColor,
                NodeType.Clothing => Settings.ClothingNodeColor,
                NodeType.CriteriaGroup => Settings.CriteriaGroupNodeColor,
                NodeType.Pose => Settings.PoseNodeColor,
                NodeType.Property => Settings.PropertyNodeColor,
                NodeType.Social => Settings.SocialNodeColor,
                _ => Settings.DefaultNodeColor,
            };
        }

        private void EndPan()
        {
            //end of pan
            if (CurrentlyInPan)
            {
                CurrentlyInPan = false;
                Explorer.Cursor = priorCursor;
            }
        }

        private void HighlightClickedNodeHandler(object sender, ClickedNodeChangeArgs e)
        {
            if (e.ClickType == ClickedNodeTypes.Highlight)
            {
                //select line in translation manager (only if we want to)
                if (!IsShiftPressed) TabManager.ActiveTranslationManager.SelectLine(e.ChangedNode.ID);
                //put info up
                Explorer.Invoke(() => DisplayNodeInfo(e.ChangedNode));
            }
        }

        private void SetPanOffset(Point location)
        {
            StartPanOffsetX = location.X;
            StartPanOffsetY = location.Y;
        }

        private Node GetClickedNode(Point mouseLocation, out int index)
        {
            //handle position input
            ScreenToGraph(mouseLocation.X, mouseLocation.Y, out float mouseLeftX, out float mouseUpperY);
            ScreenToGraph(mouseLocation.X, mouseLocation.Y, out float mouseRightX, out float mouseLowerY);

            index = -1;
            Node node;
            for (int i = 0; i < Provider.Nodes.Count; i++)
            {
                node = Provider.Nodes[i];
                if (mouseLowerY > node.Position.Y - StoryExplorerConstants.Nodesize / 2 && mouseUpperY < node.Position.Y + StoryExplorerConstants.Nodesize / 2)
                {
                    if (mouseRightX < node.Position.X + StoryExplorerConstants.Nodesize / 2 && mouseLeftX > node.Position.X - StoryExplorerConstants.Nodesize / 2)
                    {
                        if (publicNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
                        {
                            index = i;
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
                priorCursor = Explorer.Cursor;
                Explorer.Cursor = Cursors.Cross;
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

            //WHEEL_DELTA = 120, as per windows documentation
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.mouseeventargs.delta?view=windowsdesktop-6.0
            if (e.Delta > 0)
                Scaling *= 1.2f;
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

        public void TrySelectNextUp()
        {
            if (HighlightedNode.ParentNodes.Count > 0) HighlightedNode = HighlightedNode.ParentNodes[0];
        }

        public void TrySelectNextDown()
        {
            if (HighlightedNode.ChildNodes.Count > 0) HighlightedNode = HighlightedNode.ChildNodes[0];
        }
    }
}
