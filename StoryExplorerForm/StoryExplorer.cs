using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private readonly ContextProvider Context;
        public StoryExplorer(bool IsStory)
        {
            InitializeComponent();
            Context = new ContextProvider("", ExplorerPanel, IsStory, false);
            if (Context.ParseFile())
            {
                //go on displaying graph
            }
            else
            {
                //else we quit
                Close();
            }
        }
    }
}
