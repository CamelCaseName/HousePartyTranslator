using HousePartyTranslator.Managers;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private readonly ContextProvider Context;
        public GraphingEngine Grapher { get; }
        public string ParentName { get; }


        public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, Form Parent)
        {
            InitializeComponent();

            //indicate ownership
            ParentName = Parent.Name;

            Cursor = Cursors.WaitCursor;

            //change draw order for this windows from bottom to top to top to bottom to remove flickering
            //use double buffering for that
            DoubleBuffered = true;

            //get contextprovider
            Context = new ContextProvider(IsStory, AutoLoad, FileName);
            Grapher = new GraphingEngine(Context, this, NodeInfoLabel);

            Text = $"StoryExplorer - {FileName}";

            //add custom paint event handler to draw all nodes and edges
            Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);

            //parse story, and not get cancelled xD
            if (Context.ParseFile() && !Context.GotCancelled)
            {
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
            Grapher.HandleKeyBoard(sender, e);
        }

       private void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            Grapher.HandleMouseEvents(sender, e);
        }
    }
}
