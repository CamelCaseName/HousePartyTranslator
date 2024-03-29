﻿using System.Drawing;
using System.Windows.Forms;

namespace Translator.Desktop.UI
{
    public partial class ProgressWindow : Form
    {
        public ProgressWindow()
        {
            Location = new Point((Screen.PrimaryScreen?.Bounds.Size / 2) - Size ?? new Size(0, 0));
            InitializeComponent();
            noAnimationBar1.Maximum = 5;
            noAnimationBar1.Value = 0;
            noAnimationBar1.Step = 1;
            Invalidate();
            Status = label1;

            IsInitialized = true;
        }

        public Label Status
        {
            get;
        }
        public bool IsInitialized { get; internal set; }

        public void PerformStep()
        {
            noAnimationBar1.PerformStep();
            Invalidate();
            Update();
        }
    }
}
