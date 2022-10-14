using HousePartyTranslator.Helpers;
using System.Windows.Forms;

namespace HousePartyTranslator.ProgressbarForm
{
    public partial class ProgressWindow : Form
    {
        public ProgressWindow()
        {
            ProgressBar = noAnimationBar1;
            InitializeComponent();
        }

        public NoAnimationBar ProgressBar
        {
            get;
        }

    }
}
