using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private ContextProvider Context;
        private GraphingEngine engine;
        private Form parentForm;
        private string parentName;

        public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, string StoryName, Form Parent, ParallelOptions parallelOptions)
        {
            InitializeComponent();

            //indicate ownership
            parentName = Parent.Name;
            parentForm = Parent;

            //change draw order for this windows from bottom to top to top to bottom to remove flickering
            //use double buffering for that
            DoubleBuffered = true;

            //get contextprovider
            Context = new ContextProvider(IsStory, AutoLoad, FileName, StoryName, parallelOptions);
            engine = new GraphingEngine(Context, this, NodeInfoLabel);

            Text = $"StoryExplorer - {FileName}";

            //add custom paint event handler to draw all nodes and edges
            Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);

        }

        public Form FormParent { get { return parentForm; } }
        public GraphingEngine Grapher { get { return engine; } }
        public string ParentName { get { return parentName; } }

        public void Initialize()
        {
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
