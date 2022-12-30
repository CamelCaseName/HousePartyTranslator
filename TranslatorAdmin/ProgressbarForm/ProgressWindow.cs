using Translator.Helpers;

namespace Translator.ProgressbarForm
{
    public partial class ProgressWindow : Form
    {
        public ProgressWindow()
        {
            InitializeComponent();
            noAnimationBar1.Maximum = 10;
            noAnimationBar1.Value = 0;
            noAnimationBar1.Step = 1;
            ProgressBar = noAnimationBar1;
            Status = label1;

            IsInitialized = true;
        }

        public NoAnimationBar ProgressBar
        {
            get;
        }

        public Label Status
        {
            get;
        }
        public bool IsInitialized { get; internal set; }

        public void PerformStep()
        {
            noAnimationBar1.PerformStep();
            Update();
        }
    }
}
