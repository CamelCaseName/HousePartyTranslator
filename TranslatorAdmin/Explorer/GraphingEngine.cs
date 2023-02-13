using Microsoft.VisualBasic.Logging;
using System.Buffers;
using System.Reflection;
using System.Runtime.Versioning;
using Translator.Core;
using Translator.Core.Helpers;
using Translator.Explorer.Window;
using Translator.Managers;
using TranslatorDesktopApp.Explorer;
using static Translator.Explorer.JSON.StoryEnums;
using Settings = Translator.InterfaceImpls.WinSettings;
using TabManager = Translator.Core.TabManager<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;

namespace Translator.Explorer
{
    [SupportedOSPlatform("Windows")]
    internal sealed class GraphingEngine
    {
        private static float Nodesize => StoryExplorerConstants.Nodesize;
        public const float ColorFactor = 0.7f;

        public readonly NodeProvider Provider;

        private readonly SolidBrush ColorBrush;
        private readonly Pen ColorPen;

        private static float Xmax = 0;
        private static float Ymax = 0;
        private static float Xmin = 0;
        private static float Ymin = 0;

        private readonly StoryExplorer Explorer;
        private readonly Label NodeInfoLabel;

        private readonly ArrayPool<PointF> PointPool = ArrayPool<PointF>.Shared;
        private readonly Dictionary<Type, GroupBox> ExtendedInfoComponents = new();

        private float AfterZoomMouseX = 0f;
        private float AfterZoomMouseY = 0f;
        private float BeforeZoomMouseX = 0f;
        private float BeforeZoomMouseY = 0f;
        private float OffsetX;
        private float OffsetY;

        private bool CurrentlyInPan = false;
        private Node highlightedNode = Node.NullNode;
        private Node infoNode = Node.NullNode;
        private Node movingNode = Node.NullNode;
        private bool IsShiftPressed = false;
        private bool IsCtrlPressed = false;
        private bool MovingANode = false;
        public bool DrewNodes = false;
        //todo replace by settings, on screen button and allow node selection
        //todo add internal nodes visible button and setting
        public bool InternalNodesVisible = true;
        public bool ShowExtendedInfo = true;
        private HashSet<Node> NodesHighlighted = new();
        private Cursor priorCursor = Cursors.Default;

        private bool ReadOnly => !Settings.WDefault.EnableStoryExplorerEdit;

        private float Scaling = 0.3f;
        private float StartPanOffsetX = 0f;
        private float StartPanOffsetY = 0f;
        private float OldMouseMovingPosX;
        private float OldMouseMovingPosY;

        public GraphingEngine(NodeProvider provider, StoryExplorer explorer, Label nodeInfoLabel)
        {
            Provider = provider;
            Explorer = explorer;
            NodeInfoLabel = nodeInfoLabel;
            NodesHighlighted = new(Provider.Nodes.Count);

            OffsetX = (float)Explorer.ClientRectangle.X / 2;
            OffsetY = (float)Explorer.ClientRectangle.Y / 2;


            ColorBrush = new SolidBrush(Settings.WDefault.DefaultNodeColor);
            ColorPen = new Pen(Settings.WDefault.DefaultEdgeColor, 2f) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor, StartCap = System.Drawing.Drawing2D.LineCap.Round };

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

        public void DrawNodesPaintHandler(object? sender, PaintEventArgs? e)
        {
            if (e != null)
            {
                //set up values for this paint cycle
                Xmin = 2 * -Nodesize;
                Ymin = 2 * -Nodesize;
                Xmax = App.MainForm.Explorer?.Size.Width ?? 0 + Nodesize;
                Ymax = App.MainForm.Explorer?.Size.Height ?? 0 + Nodesize;

                //disables and reduces unused features
                e.Graphics.ToLowQuality();
                e.Graphics.TranslateTransform(-OffsetX * Scaling, -OffsetY * Scaling);
                e.Graphics.ScaleTransform(Scaling, Scaling);

                PaintAllNodes(e.Graphics);

                //overlay info and highlight
                DrawHighlightNodeTree(e.Graphics);
                DrawMovingNodeMarker(e.Graphics);
                DrawInfoNode(e.Graphics);
                NodeInfoLabel.Invalidate();
                NodeInfoLabel.Update();
            }
        }

        private void DrawMovingNodeMarker(Graphics g)
        {
            if (movingNode != Node.NullNode)
            {
                DrawColouredNode(g, movingNode, Settings.WDefault.MovingNodeColor, 1.2f);
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
            {
                BeginMovingNodeMovement(MouseLocation);
            }
            if (IsCtrlPressed && MovingANode && movingNode != Node.NullNode)
            {
                UpdateMovingNodeMovement(MouseLocation);
            }
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
            //go on displaying graph
            for (int i = 0; i < Provider.Nodes.Count; i++)
            {
                DrawColouredNode(g, Provider.Nodes[i]);
            }
            int maxEdges = Math.Min(Provider.Nodes.Edges.Count, Settings.WDefault.MaxEdgeCount);
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
                string info;
                if (Settings.WDefault.DisplayVAHints) { info = node.Text.ConstrainLength(); }
                else { info = node.Text.RemoveVAHints().ConstrainLength(); }

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
                foreach (var box in ExtendedInfoComponents.Values)
                {
                    box.Visible = false;
                }
                NodeInfoLabel.Visible = false;
            }

            Explorer.Invalidate();
        }

        private void DisplayExtendedNodeInfo(Rectangle infoLabelRect, Node node)
        {
            foreach (var oldBox in ExtendedInfoComponents.Values)
            {
                oldBox.Visible = false;
            }
            if (node.Data == null) return;

            //use components if we have them already
            if (!ExtendedInfoComponents.TryGetValue(node.DataType, out var box))
            {
                //create new and cache, then display
                box = GetDisplayComponentsForType(node.Data, node.DataType);
                Explorer.Controls.Add(box);
                ExtendedInfoComponents.Add(node.DataType, box);
            }
            else
            {
                FillDisplayComponents(box, node.Data);
            }
            box.Location = new Point(infoLabelRect.Left, infoLabelRect.Bottom + 5);
            box.MaximumSize = new Size(Math.Clamp(infoLabelRect.Width, 340, 900), Explorer.ClientRectangle.Height - Explorer.Grapher.NodeInfoLabel.Height - 10);
            box.Size = new Size(Math.Clamp(infoLabelRect.Width, 340, 900), 10);
            SetEditableStates(box);
            box.BringToFront();
            box.Visible = true;
        }

        private void SetEditableStates(GroupBox box)
        {
            foreach (var control in box.Controls[0].Controls)
            {
                if (control.GetType() == typeof(ComboBox) || control.GetType() == typeof(CheckBox)) ((Control)control).Enabled = !ReadOnly;
                else if (control.GetType() == typeof(TextBox)) ((TextBox)control).ReadOnly = ReadOnly;
                else if (control.GetType() == typeof(NumericUpDown)) ((NumericUpDown)control).ReadOnly = ReadOnly;
            }
        }

        private void FillDisplayComponents(GroupBox box, object data)
        {
            foreach (var property in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                Type valueType = property.PropertyType;
                object? value = property.GetValue(data);

                if (valueType == typeof(string))
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "TextBox", true);
                    if (text.Length == 1) text[0].Text = (string?)value ?? string.Empty;
                }
                else if (valueType == typeof(int) || valueType == typeof(float))
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "Numeric", true);
                    if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(NumericUpDown))) ((NumericUpDown)text[0]).Value = Convert.ToDecimal(value);
                }
                else if (valueType == typeof(bool))
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "CheckBox", true);
                    if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(CheckBox))) ((CheckBox)text[0]).Checked = Convert.ToBoolean(value);
                }
                else if (valueType.GenericTypeArguments.Length > 0)
                {
                    if (valueType.GenericTypeArguments[0].IsEnum)
                    {
                        var text = box.Controls[0].Controls.Find(property.Name + "ComboBox", true);
                        if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(ComboBox))) ((ComboBox)text[0]).SelectedItem = value?.ToString();
                    }
                }
                else if (valueType.IsEnum)
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "ComboBox", true);
                    if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(ComboBox))) ((ComboBox)text[0]).SelectedItem = value?.ToString();
                }
            }
        }

        private GroupBox GetDisplayComponentsForType(object data, Type dataType)
        {
            var box = new GroupBox()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                AutoSize = true,
                ForeColor = Utils.brightText,
                Name = dataType.Name + "DisplayBox",
                TabIndex = 10,
                TabStop = false,
                MaximumSize = new Size(Math.Clamp(NodeInfoLabel.ClientRectangle.Width, 340, 900), Explorer.ClientRectangle.Height - Explorer.Grapher.NodeInfoLabel.Height - 10),
                Text = dataType.Name,
                Visible = false
            };
            box.SuspendLayout();
            var grid = new TableLayoutPanel()
            {
                Name = dataType.Name + "ValueTable",
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                AutoSize = true,
                TabStop = false,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 2,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Dock = DockStyle.Fill,
                AutoScroll = true,
            };
            box.Controls.Add(grid);

            foreach (var property in dataType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    Type valueType = property.PropertyType;
                    object? value = property.GetValue(data);
                    //string
                    if (valueType == typeof(string))
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var text = new TextBox() { Text = (string?)value ?? string.Empty, Name = property.Name + "TextBox", Multiline = true, WordWrap = true, Dock = DockStyle.Fill, ReadOnly = ReadOnly, };
                        text.TextChanged += (object? sender, EventArgs e) => TextBoxSetValue(sender, property, data);
                        grid.Controls.Add(text);
                    }
                    //numbers
                    else if (valueType == typeof(int) || valueType == typeof(float))
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var numeric = new NumericUpDown() { Minimum = int.MinValue, Maximum = int.MaxValue, Value = Convert.ToDecimal(value), Name = property.Name + "Numeric", ReadOnly = ReadOnly, InterceptArrowKeys = true };
                        if (valueType == typeof(int)) numeric.ValueChanged += (object? sender, EventArgs e) => NumericIntSetValue(sender, property, data);
                        else numeric.ValueChanged += (object? sender, EventArgs e) => NumericFloatSetValue(sender, property, data);
                        grid.Controls.Add(numeric);
                    }
                    //bool
                    else if (valueType == typeof(bool))
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var checkBox = new CheckBox { Checked = Convert.ToBoolean(value), Name = property.Name + "Checkbox", Enabled = !ReadOnly };
                        checkBox.CheckedChanged += (object? sender, EventArgs e) => CheckBoxSetValue(sender, property, data);
                        grid.Controls.Add(checkBox);
                    }
                    //nullable enum
                    else if (valueType.GenericTypeArguments.Length > 0)
                    {
                        if (valueType.GenericTypeArguments[0].IsEnum)
                        {
                            var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                            grid.Controls.Add(label);
                            var dropDown = new ComboBox() { Name = property.Name + "ComboBox", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left };
                            foreach (var enumItem in Enum.GetNames(valueType.GenericTypeArguments[0]))
                            {
                                dropDown.Items.Add(enumItem);
                            }
                            dropDown.SelectedItem = value?.ToString();
                            dropDown.SelectedValueChanged += (object? sender, EventArgs e) => DropDownNullableSetValue(sender, property, data);
                            dropDown.Enabled = !ReadOnly;
                            grid.Controls.Add(dropDown);
                        }
                    }
                    //enum
                    else if (valueType.IsEnum)
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var dropDown = new ComboBox() { Name = property.Name + "ComboBox", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left };
                        foreach (var enumItem in Enum.GetNames(valueType))
                        {
                            dropDown.Items.Add(enumItem);
                        }
                        dropDown.SelectedItem = value?.ToString();
                        dropDown.SelectedValueChanged += (object? sender, EventArgs e) => DropDownSetValue(sender, property, data);
                        dropDown.Enabled = !ReadOnly;
                        grid.Controls.Add(dropDown);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Log(e.Message);
                }
            }

            box.ResumeLayout();
            return box;
        }

        private void DropDownSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Enum.Parse(property.PropertyType, ((ComboBox)sender).SelectedItem?.ToString()!));
        }

        private void DropDownNullableSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Enum.Parse(property.PropertyType.GenericTypeArguments[0], ((ComboBox)sender).SelectedItem?.ToString() ?? string.Empty));
        }

        private void NumericIntSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Convert.ToInt32(((NumericUpDown)sender).Value));
        }

        private void NumericFloatSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, (float)Convert.ToDouble(((NumericUpDown)sender).Value));
        }

        private void TextBoxSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, ((TextBoxBase)sender).Text);
        }

        private void CheckBoxSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Convert.ToBoolean(((CheckBox)sender).Checked));
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
            //todo replace this call by one in the beginning of the paint event, then use the cached vaslues in graph space for comparison. should save up to 200k method calls
            //dont draw node if it is too far away
            GraphToScreen(node.Position.X, node.Position.Y, out float x, out float y);
            if (x <= Xmax && y <= Ymax && x >= Xmin && y >= Ymin)
            {
                if (InternalNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
                {
                    ColorBrush.Color = color;
                    g.FillEllipse(
                        ColorBrush,
                        node.Position.X - (Nodesize / 2) * scale,
                        node.Position.Y - (Nodesize / 2) * scale,
                        Nodesize * scale,
                        Nodesize * scale
                        );
                }
            }
        }

        internal void DrawEdge(Graphics g, Node node1, Node node2)
        {
            DrawEdge(g, node1, node2, Settings.WDefault.DefaultEdgeColor);
        }

        internal void DrawEdge(Graphics g, Node node1, Node node2, Color color, float width = 1.5f)
        {
            if (InternalNodesVisible || (node1.Type != NodeType.Event && node1.Type != NodeType.Criterion && node2.Type != NodeType.Event && node2.Type != NodeType.Criterion))
            {
                //todo same as with the points, get the bounds of the screens and then remove these calls here
                //so we can do the bounds checking with cached values and dont need these method calls
                //dont draw node if it is too far away
                GraphToScreen(node1.Position.X, node1.Position.Y, out float x1, out float y1);
                GraphToScreen(node2.Position.X, node2.Position.Y, out float x2, out float y2);

                //sort out lines that would be too small on screen and ones where none of the ends are visible
                //todo swap around such that we just compare length difference to max edge length, no more sqrt
                if (MathF.Sqrt(MathF.Pow(x1 - x2, 2) + MathF.Pow(y1 - y2, 2)) > 10 &&
                    ((x1 <= Xmax && y1 <= Ymax && x1 >= Xmin && y1 >= Ymin) ||
                    (x2 <= Xmax && y2 <= Ymax && x2 >= Xmin && y2 >= Ymin)))
                {
                    //any is visible
                    ColorPen.Color = color;
                    ColorPen.Width = width;
                    //todo adjust edge offsets to only touch the nodes in future (if only for small graphs so we dont compromise performance)?
                    g.DrawLine(
                        ColorPen,
                        node1.Position.X,
                        node1.Position.Y,
                        node2.Position.X,
                        node2.Position.Y
                        );
                }
                //none are visible, why draw?
            }
        }

        internal void DrawEdges(Graphics g, NodeList nodes)
        {
            DrawEdges(g, nodes, Settings.WDefault.DefaultEdgeColor);
        }

        internal void DrawEdges(Graphics g, NodeList nodes, Color color)
        {
            ColorPen.Color = color;
            PointF[] points = PointPool.Rent(nodes.Edges.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                points[i] = nodes[i].Position;
            }
            g.DrawLines(ColorPen, points);
            PointPool.Return(points);
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
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
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
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
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
            {
                DrawColouredNode(g, InfoNode, Settings.WDefault.InfoNodeColor);
            }
        }

        private void DrawHighlightNodeSet(Graphics g, Node node, int depth, int maxDepth, Color nodeColor, Color edgeColor)
        {
            _ = NodesHighlighted.Add(node);

            //draw node 
            if (depth != 0)
                if (Settings.WDefault.UseRainbowNodeColors)
                    DrawColouredNode(g, node, nodeColor);
                else
                    DrawColouredNode(g, node, ColorFromNode(node));

            if (depth++ < maxDepth)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (!NodesHighlighted.Contains(node.ChildNodes[i]))
                    {
                        DrawHighlightNodeSet(g, node.ChildNodes[i], depth, maxDepth, Rainbow((float)depth / 10), RainbowEdge((float)depth / 14));
                    }
                    if (Settings.WDefault.UseRainbowEdgeColors)
                        DrawEdge(g, node, node.ChildNodes[i], edgeColor);
                    else
                        DrawEdge(g, node, node.ChildNodes[i], Color.LightGray, 2f);
                }
                for (int i = 0; i < node.ParentNodes.Count; i++)
                {
                    if (!NodesHighlighted.Contains(node.ParentNodes[i]))
                    {
                        DrawHighlightNodeSet(g, node.ParentNodes[i], depth, maxDepth, Rainbow((float)depth / 10), RainbowEdge((float)depth / 14));
                    }
                    if (Settings.WDefault.UseRainbowEdgeColors)
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
                NodeType.Null => Settings.WDefault.DefaultNodeColor,
                NodeType.Item => Settings.WDefault.ItemNodeColor,
                NodeType.ItemGroup => Settings.WDefault.ItemGroupNodeColor,
                NodeType.ItemAction => Settings.WDefault.ActionNodeColor,
                NodeType.Event => Settings.WDefault.EventNodeColor,
                NodeType.Criterion => Settings.WDefault.CriterionNodeColor,
                NodeType.Response => Settings.WDefault.ResponseNodeColor,
                NodeType.Dialogue => node.Gender switch
                {
                    Gender.Female => Settings.WDefault.DialogueFemaleOnlyNodeColor,
                    Gender.Male => Settings.WDefault.DialogueMaleOnlyNodeColor,
                    _ => Settings.WDefault.DialogueNodeColor
                },
                NodeType.Quest => Settings.WDefault.QuestNodeColor,
                NodeType.Achievement => Settings.WDefault.AchievementNodeColor,
                NodeType.EventTrigger => Settings.WDefault.ReactionNodeColor,
                NodeType.BGC => Settings.WDefault.BGCNodeColor,
                NodeType.Value => Settings.WDefault.ValueNodeColor,
                NodeType.Door => Settings.WDefault.DoorNodeColor,
                NodeType.Inventory => Settings.WDefault.InventoryNodeColor,
                NodeType.State => Settings.WDefault.StateNodeColor,
                NodeType.Personality => Settings.WDefault.PersonalityNodeColor,
                NodeType.Cutscene => Settings.WDefault.CutsceneNodeColor,
                NodeType.Clothing => Settings.WDefault.ClothingNodeColor,
                NodeType.CriteriaGroup => Settings.WDefault.CriteriaGroupNodeColor,
                NodeType.Pose => Settings.WDefault.PoseNodeColor,
                NodeType.Property => Settings.WDefault.PropertyNodeColor,
                NodeType.Social => Settings.WDefault.SocialNodeColor,
                _ => Settings.WDefault.DefaultNodeColor,
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
                //tell translationmanager to update us or not when selected
                WinTranslationManager.UpdateStoryExplorerSelection = !IsShiftPressed;
                //select line in translation manager
                TabManager.ActiveTranslationManager.SelectLine(e.ChangedNode.ID);
                //put info up
                highlightedNode = e.ChangedNode;
                if (Provider.Nodes.Count != NodesHighlighted.Count) NodesHighlighted = new(Provider.Nodes.Count);
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
                if (mouseLowerY > node.Position.Y - (Nodesize / 2) && mouseUpperY < node.Position.Y + (Nodesize / 2))
                {
                    if (mouseRightX < node.Position.X + (Nodesize / 2) && mouseLeftX > node.Position.X - (Nodesize / 2))
                    {
                        if (InternalNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
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
