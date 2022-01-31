using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using System.Drawing;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private readonly ContextProvider Context;
        private readonly GraphingManager Grapher;
        private bool CurrentlyInPan = false;
        int selector = -1;

        //TODO move calculations to different file first, thread later?

        //TODO make colouring function recursive and automatable in use (depth and colour)

        public StoryExplorer(bool IsStory, bool AutoLoad, string FileName)
        {
            InitializeComponent();

            Cursor = Cursors.WaitCursor;
            //set offset

            //change draw order for this windows from bottom to top to top to bottom to remove flickering
            //use double buffering for that
            DoubleBuffered = true;

            //get contextprovider
            Context = new ContextProvider(IsStory, AutoLoad, FileName);
            Grapher = new GraphingManager(Context, this);

            //add custom paint event handler to draw all nodes and edges
            Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);

            //parse story, and not get cancelled xD
            if (Context.ParseFile() && !Context.GotCancelled)
            {
                Grapher.NodeToHighlight = Context.GetNodes()[0];
                Grapher.PaintAllNodes();
            }
            else
            {
                //else we quit
                Close();
            }
        }


        private void HandleKeyBoard(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                Grapher.DrawOverHighlight();
                selector++;
                Grapher.NodeToHighlight = Context.GetNodes()[selector];
                Grapher.DrawHighlightNodeTree();
            }
            else if (e.KeyCode == Keys.D)
            {
                Grapher.DrawOverHighlight();
                if (selector > 0) selector--;
                Grapher.NodeToHighlight = Context.GetNodes()[selector];
                Grapher.DrawHighlightNodeTree();
            }
        }

        private void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    //handle position input
                    Grapher.ScreenToGraph(e.X - GraphingManager.Nodesize, e.Y - GraphingManager.Nodesize, out float mouseLeftX, out float mouseUpperY);
                    Grapher.ScreenToGraph(e.X + GraphingManager.Nodesize, e.Y + GraphingManager.Nodesize, out float mouseRightX, out float mouseLowerY);

                    foreach (Node node in Context.GetNodes())
                    {
                        if (mouseLowerY > node.Position.Y && mouseUpperY < node.Position.Y)
                        {
                            if (mouseRightX > node.Position.X && mouseLeftX < node.Position.X)
                            {
                                Grapher.DrawOverHighlight();
                                Grapher.NodeToHighlight = node;
                                Grapher.DrawHighlightNodeTree();
                            }
                        }
                    }
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
                        Grapher.SetPanOffset(e.Location);
                    }
                    //in pan
                    else if (CurrentlyInPan)
                    {
                        Grapher.UpdatePanOffset(e.Location);

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
                Grapher.UpdateScaling(e);

                //redraw
                Invalidate();
            }
        }
    }
}
