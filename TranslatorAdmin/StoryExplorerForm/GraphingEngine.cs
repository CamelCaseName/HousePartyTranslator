using System.Buffers;
using System.Runtime.Versioning;
using Translator.Core.Helpers;
using Translator.Explorer.Window;
using TranslatorAdmin.Managers;
using static Translator.Explorer.JSON.StoryEnums;
using Settings = TranslatorAdmin.InterfaceImpls.WinSettings;
using TabManager = Translator.Core.TabManager<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController, TranslatorAdmin.InterfaceImpls.WinTab>;

namespace Translator.Explorer
{
	[SupportedOSPlatform("Windows")]
	internal sealed class GraphingEngine
	{
		public const int Nodesize = 16;

		public readonly ContextProvider Context;

		private readonly Color DefaultEdgeColor = Color.FromArgb(30, 30, 30);
		private readonly Color DefaultMaleColor = Color.Coral;
		private readonly Color DefaultColor = Color.DarkBlue;
		private readonly Color DefaultQuestColor = Color.Purple;
		private readonly Color DefaultFemaleColor = Color.DarkTurquoise;

		private readonly SolidBrush NodeBrush;
		private readonly SolidBrush FemaleNodeBrush;
		private readonly SolidBrush MaleNodeBrush;
		private readonly SolidBrush QuestNodeBrush;
		private readonly SolidBrush ColorBrush;
		private readonly Pen DefaultPen;
		private readonly Pen ColorPen;

		private static float Xmax => App.MainForm.Explorer?.Size.Width ?? 0 + Nodesize;
		private static float Ymax => App.MainForm.Explorer?.Size.Height ?? 0 + Nodesize;
		private const float Xmin = 2 * -Nodesize;
		private const float Ymin = 2 * -Nodesize;

		private readonly StoryExplorer Explorer;
		private readonly Label NodeInfoLabel;

		private readonly ArrayPool<PointF> PointPool = ArrayPool<PointF>.Shared;

		private float AfterZoomMouseX = 0f;
		private float AfterZoomMouseY = 0f;
		private float BeforeZoomMouseX = 0f;
		private float BeforeZoomMouseY = 0f;
		private float OffsetX = 0f;
		private float OffsetY = 0f;

		private bool CurrentlyInPan = false;
		private Node highlightedNode = Node.NullNode;
		private Node infoNode = Node.NullNode;
		private bool IsShiftPressed = false;
		public bool DrewNodes = false;
		public bool InternalNodesVisible = true;

		private float Scaling = 0.3f;
		private float StartPanOffsetX = 0f;
		private float StartPanOffsetY = 0f;


		public GraphingEngine(ContextProvider context, StoryExplorer explorer, Label nodeInfoLabel)
		{
			Context = context;
			Explorer = explorer;
			NodeInfoLabel = nodeInfoLabel;

			NodeBrush = new SolidBrush(DefaultColor);
			FemaleNodeBrush = new SolidBrush(DefaultFemaleColor);
			MaleNodeBrush = new SolidBrush(DefaultMaleColor);
			QuestNodeBrush = new SolidBrush(DefaultQuestColor);
			ColorBrush = new SolidBrush(DefaultColor);
			DefaultPen = new Pen(DefaultEdgeColor, 3f);
			ColorPen = new Pen(DefaultEdgeColor, 3f);

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
				//e.Graphics.ExcludeClip(NodeInfoLabel.Bounds);

				e.Graphics.TranslateTransform(-OffsetX * Scaling, -OffsetY * Scaling);
				e.Graphics.ScaleTransform(Scaling, Scaling);

				PaintAllNodes(e.Graphics);

				//overlay info and highlight
				DrawHighlightNodeTree(e.Graphics);
				DrawInfoNode(e.Graphics);
				NodeInfoLabel.Invalidate();
				NodeInfoLabel.Update();
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
		}

		public void HandleMouseEvents(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					HighlightedNode = UpdateClickedNode(e.Location);
					break;
				case MouseButtons.None:
					EndPan();
					break;
				case MouseButtons.Right:
					InfoNode = UpdateClickedNode(e.Location);
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

		public void PaintAllNodes(Graphics g)
		{
			DrewNodes = false;
			//go on displaying graph
			for (int i = 0; i < Context.Nodes.Count; i++)
			{
				//draw edges to children, default colour
				for (int j = 0; j < Context.Nodes[i].ChildNodes.Count; j++)
				{
					DrawEdge(g, Context.Nodes[i], Context.Nodes[i].ChildNodes[j]);
				}

				DrawColouredNode(g, Context.Nodes[i]);
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
				if (((Settings)Settings.Default).DisplayVAHints) { info = node.Text.ConstrainLength(); }
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
				infoNode = e.ChangedNode;
				DisplayNodeInfo(e.ChangedNode);
			}
		}

		private void DrawColouredNode(Graphics g, Node node)
		{
			//dont draw node if it is too far away
			GraphToScreen(node.Position.X, node.Position.Y, out float x, out float y);
			if (x <= Xmax && y <= Ymax && x >= Xmin && y >= Ymin)
			{
				//todo replace filter by a prefiltered list, maybe do two seperate compination methods
				if (InternalNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
				{
					g.FillEllipse(
						BrushFromNode(node),
						node.Position.X - Nodesize / 2,
						node.Position.Y - Nodesize / 2,
						Nodesize,
						Nodesize
						);
				}
			}
		}

		private void DrawColouredNode(Graphics g, Node node, Color color)
		{
			//dont draw node if it is too far away
			GraphToScreen(node.Position.X, node.Position.Y, out float x, out float y);
			if (x <= Xmax && y <= Ymax && x >= Xmin && y >= Ymin)
			{
				if (InternalNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
				{
					ColorBrush.Color = color;
					g.FillEllipse(
						ColorBrush,
						node.Position.X - Nodesize / 2,
						node.Position.Y - Nodesize / 2,
						Nodesize,
						Nodesize
						);
				}
			}
		}

		internal void DrawEdge(Graphics g, Node node1, Node node2)
		{
			if (InternalNodesVisible || node1.Type != NodeType.Event && node1.Type != NodeType.Criterion && node2.Type != NodeType.Event && node2.Type != NodeType.Criterion)
			{
				//dont draw node if it is too far away
				GraphToScreen(node1.Position.X, node1.Position.Y, out float x1, out float y1);
				GraphToScreen(node2.Position.X, node2.Position.Y, out float x2, out float y2);

				if ((x1 <= Xmax && y1 <= Ymax && x1 >= Xmin && y1 >= Ymin) || (x2 <= Xmax && y2 <= Ymax && x2 >= Xmin && y2 >= Ymin))
				{
					//any is visible
					g.DrawLine(
						DefaultPen,
						node1.Position.X,
						node1.Position.Y,
						node2.Position.X,
						node2.Position.Y
						);
				}
				//none are visible, why draw?
			}
		}

		internal void DrawEdge(Graphics g, Node node1, Node node2, Color color)
		{
			if (InternalNodesVisible || node1.Type != NodeType.Event && node1.Type != NodeType.Criterion && node2.Type != NodeType.Event && node2.Type != NodeType.Criterion)
			{
				//dont draw node if it is too far away
				GraphToScreen(node1.Position.X, node1.Position.Y, out float x1, out float y1);
				GraphToScreen(node2.Position.X, node2.Position.Y, out float x2, out float y2);

				if ((x1 <= Xmax && y1 <= Ymax && x1 >= Xmin && y1 >= Ymin) || (x2 <= Xmax && y2 <= Ymax && x2 >= Xmin && y2 >= Ymin))
				{
					//any is visible
					ColorPen.Color = color;
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

		internal void DrawEdges(Graphics g, List<Node> nodes)
		{
			var points = PointPool.Rent(nodes.Count);
			for (int i = 0; i < nodes.Count; i++)
			{
				points[i] = nodes[i].Position;
			}
			g.DrawLines(DefaultPen, points);
			PointPool.Return(points);
		}

		internal void DrawEdges(Graphics g, List<Node> nodes, Color color)
		{
			ColorPen.Color = color;
			var points = PointPool.Rent(nodes.Count);
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
				//draw parents first (one layer only)
				DrawNodeSet(
					g,
					HighlightedNode.ParentNodes,
					HighlightedNode,
					0,
					1,
					Color.White,
					Color.DarkOrchid,
					false);
				//then childs
				DrawNodeSet(
					g,
					HighlightedNode.ChildNodes,
					HighlightedNode,
					0,
					6,
					Color.Red,
					Color.DeepPink,
					true);

				//then redraw node itself
				DrawColouredNode(g, HighlightedNode, Color.DarkRed);
				Explorer.Invalidate();
			}
		}

		private void DrawInfoNode(Graphics g)
		{
			if (InfoNode != Node.NullNode)
			{
				ColorBrush.Color = Color.ForestGreen;
				g.FillEllipse(
					ColorBrush,
					InfoNode.Position.X - Nodesize / 2,
					InfoNode.Position.Y - Nodesize / 2,
					Nodesize,
					Nodesize
					);
			}
		}

		private void DrawNodeSet(Graphics g, List<Node> nodes, Node previousNode, int depth, int maxDepth, Color nodeColor, Color edgeColor, bool diluteColor)
		{
			if (depth++ < maxDepth)
			{
				//highlight childs
				for (int i = 0; i < nodes.Count; i++)
				{
					if (diluteColor)
					{
						DrawNodeSet(g, nodes[i].ChildNodes, nodes[i], depth, maxDepth, Color.FromArgb((int)(ColorFromNode(nodes[i]).ToArgb() / 1.3d)), Color.FromArgb((int)(edgeColor.ToArgb() / 1.3d)), true);
					}
					else
					{
						DrawNodeSet(g, nodes[i].ChildNodes, nodes[i], depth, maxDepth, nodeColor, edgeColor, false);
					}

					//draw line
					DrawEdge(g, nodes[i], previousNode, edgeColor);
					//draw node over line
					DrawColouredNode(g, nodes[i], nodeColor);
				}
			}
		}

		private Color ColorFromNode(Node node)
		{
			return node.Gender switch
			{
				Gender.None => node.Type == NodeType.Quest ? DefaultQuestColor : DefaultColor,
				Gender.Female => DefaultFemaleColor,
				Gender.Male => DefaultMaleColor,
				_ => DefaultColor,
			};
		}

		private SolidBrush BrushFromNode(Node node)
		{
			return node.Gender switch
			{
				Gender.None => node.Type == NodeType.Quest ? QuestNodeBrush : NodeBrush,
				Gender.Female => FemaleNodeBrush,
				Gender.Male => MaleNodeBrush,
				_ => NodeBrush,
			};
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
				//tell translationmanager to update us or not when selected
				WinTranslationManager.UpdateStoryExplorerSelection = !IsShiftPressed;
				//select line in translation manager
				TabManager.ActiveTranslationManager.SelectLine(e.ChangedNode.ID);
				//put info up
				highlightedNode = e.ChangedNode;
				DisplayNodeInfo(e.ChangedNode);
			}
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
						if (InternalNodesVisible || node.Type != NodeType.Event && node.Type != NodeType.Criterion)
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
