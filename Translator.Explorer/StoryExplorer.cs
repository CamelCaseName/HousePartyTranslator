using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Desktop;
using Translator.Desktop.UI.Components;
using Translator.Explorer.Graph;
using Translator.Explorer.Story;
using Settings = Translator.Desktop.InterfaceImpls.WinSettings;

namespace Translator.Explorer.Window
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class StoryExplorer : Form
    {
        private readonly ContextProvider Context;
        private readonly GraphingEngine engine;
        private readonly string parentName;
        public readonly string FileName;
        public readonly string StoryName;
        private bool MenuVisible = false;
        private bool inInitialization = true;
        public const string Version = "1.2.4.0";
        public const string Title = "StoryExplorer v" + Version;
        private readonly CancellationToken token;
        public NodeLayout? Layouter { get; private set; }
        public GraphingEngine Grapher { get { return engine; } }
        public NodeProvider Provider { get; }
        public string ParentName { get { return parentName; } }

        //todo implement node saving to a story file
        public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, string StoryName, Form Parent, CancellationToken cancellation)
        {
            InitializeComponent();
            InitializeTypeFilterButtons();
            token = cancellation;

            //indicate ownership
            parentName = Parent.Name;

            //change draw order for this windows from bottom to top to top to bottom to remove flickering
            //use double buffering for that
            DoubleBuffered = true;

            //get contextprovider
            Provider = new();
            Context = new(Provider, IsStory, AutoLoad, FileName, StoryName);
            engine = new(Provider, this, NodeInfoLabel);
            ShowExtendedInfo.Checked = GraphingEngine.ShowExtendedInfo;

            //if user cancels during file selection
            if (Context.FileName == "character" || Context.StoryName == "story") Close();

            this.StoryName = Context.StoryName;
            this.FileName = Context.FileName;

            Text = Title + " - Loading";

            //add custom paint event handler to draw all nodes and edges
            Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);
            FormClosing += new FormClosingEventHandler(SaveNodes);

            ColoringDepth.Value = StoryExplorerConstants.ColoringDepth = Settings.ColoringDepth;
            IdealLength.Value = (decimal)(StoryExplorerConstants.IdealLength = Settings.IdealLength);
            NodeSizeField.Value = StoryExplorerConstants.Nodesize;
        }

        private void InitializeTypeFilterButtons()
        {
            NodeType[] values = Enum.GetValues<NodeType>();
            for (int i = 0; i < values.Length; i++)
            {
                NodeType type = values[i];
                var typeButton = new ToggleButton()
                {
                    Anchor = AnchorStyles.Top,
                    AutoSize = true,
                    BackColor = SystemColors.ButtonFace,
                    ForeColor = SystemColors.MenuText,
                    Enabled = false,
                    Margin = Padding.Empty,
                    Name = type.ToString() + "Button",
                    Text = type.ToString(),
                    UseVisualStyleBackColor = true,
                };
                typeButton.Click += (object? sender, EventArgs e) =>
                {
                    if (typeButton.IsChecked) Provider.AddFilter(type);
                    else Provider.RemoveFilter(type);
                    Provider.ApplyFilters();
                };
                NodeTypeButtonsLayout.Controls.Add(typeButton);
            }
        }

        public void Initialize(bool singleFile)
        {
            if (singleFile)
            {
                App.MainForm.Invoke(() => Text = Title + " - waiting");
                if (!Context.ParseFile() || Context.GotCancelled)
                {
                    Close();
                }
                App.MainForm.Invoke(() => Text = Title + $" - {FileName}");
            }
            else
            {
                App.MainForm.Invoke(() => Text = Title + " - waiting");
                //parse story, and not get cancelled xD
                if (!Context.ParseAllFiles() || Context.GotCancelled)
                {
                    Close();
                }
                App.MainForm.Invoke(() => Text = Title + $" - {StoryName}");
            }
            inInitialization = false;
            App.MainForm.Invoke(() => NodeCalculations.Text = "Calculation running");

            Provider.FreezeNodesAsInitial();
            Layouter = new(Provider, this, token);
            SetTypesAvailable(Provider.Nodes.Types.Keys);
            Layouter.Start();
            Grapher.StartTime = DateTime.UtcNow;
            Grapher.Center();
            Invalidate();
        }

        private void SaveNodes(object? sender, FormClosingEventArgs? e)
        {
            Layouter?.Stop();
            _ = Context.SaveNodes(Provider.GetPositions());
            LogManager.Log($"\trendering ended, calculated {Grapher.FrameCount} frames in {(Grapher.FrameEndTime - Grapher.StartTime).TotalSeconds:F2} seconds -> {Grapher.FrameCount / (Grapher.FrameEndTime - Grapher.StartTime).TotalSeconds:F2} fps");
            //save story objecs here
            Settings.Default.Save();
        }

        private void HandleKeyBoard(object sender, KeyEventArgs e)
        {
            Grapher.HandleKeyBoard(sender, e);
        }

        private void HandleMouseEvents(object sender, MouseEventArgs e)
        {
            Grapher.HandleMouseEvents(sender, e);
        }

        private void Start_Click(object sender, EventArgs e)
        {
            Layouter?.Start();
            ShowRunningInfoLabel();
        }

        public void ShowRunningInfoLabel()
        {
            NodeCalculations.Text = "Calculation running";
            NodeCalculations.Update();
            NodeCalculations.Invalidate();
        }

        public void Stop_Click(object sender, EventArgs e)
        {
            if (Layouter?.Started ?? false) Layouter?.Stop();
            ShowStoppedInfoLabel();
        }

        public void ShowStoppedInfoLabel()
        {
            NodeCalculations.Text = "Calculation stopped";
            NodeCalculations.Update();
            NodeCalculations.Invalidate();
        }

        private void IdealLength_ValueChanged(object sender, EventArgs e)
        {
            if (!inInitialization)
            {
                StoryExplorerConstants.IdealLength = IdealLength.Value > 1 && IdealLength.Value < IdealLength.Maximum ? (float)IdealLength.Value : StoryExplorerConstants.IdealLength;
                Settings.IdealLength = (float)IdealLength.Value;
            }
        }

        private void ColoringDepth_ValueChanged(object sender, EventArgs e)
        {
            if (!inInitialization)
            {
                StoryExplorerConstants.ColoringDepth = ColoringDepth.Value > 1 && ColoringDepth.Value < ColoringDepth.Maximum ? (int)ColoringDepth.Value : StoryExplorerConstants.ColoringDepth;
                Settings.ColoringDepth = (int)ColoringDepth.Value;
            }
        }

        private void MenuShowButton_Click(object sender, EventArgs e)
        {
            MenuVisible = !MenuVisible;
            MenuBox.Visible = MenuVisible;
            MenuShowButton.Text = MenuVisible ? "Hide Menu" : "Show Menu";
            MenuBox.Update();
            MenuShowButton.Update();
            MenuBox.Invalidate();
            MenuShowButton.Invalidate();
        }

        private void NodeSizeField_ValueChanged(object sender, EventArgs e)
        {
            if (!inInitialization)
            {
                StoryExplorerConstants.Nodesize = NodeSizeField.Value > 1 && NodeSizeField.Value < NodeSizeField.Maximum ? (int)NodeSizeField.Value : StoryExplorerConstants.Nodesize;
            }
        }

        private void TextNodesOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (TextNodesOnly.Checked)
            {
                Provider.ApplyDefaultFilter();
            }
            else
            {
                Provider.ResetFilters();
            }
        }

        private void ShowExtendedInfo_CheckedChanged(object sender, EventArgs e)
        {
            GraphingEngine.ShowExtendedInfo = ShowExtendedInfo.Checked;
        }

        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            Grapher.TrySelectNextUp();
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            Grapher.TrySelectNextDown();
        }

        private void CenterButton_Click(object sender, EventArgs e)
        {
            Grapher.Center();
        }

        public void SetNextButtonStates(bool parentAvailable, bool ChildAvailable)
        {
            MoveUpButton.Enabled = parentAvailable;
            MoveDownButton.Enabled = ChildAvailable;
        }

        public void SetTypesAvailable(IEnumerable<NodeType> availableTypes)
        {
            for (int i = 0; i < NodeTypeButtonsLayout.Controls.Count; i++)
            {
                NodeTypeButtonsLayout.Controls[i].Enabled = false;
            }

            Provider.TextOnlyEvents = true;

            //cache so we can filter while acessing old types
            var enumerator = new List<NodeType>(availableTypes).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Control? button = (ToggleButton?)NodeTypeButtonsLayout.Controls[enumerator.Current.ToString() + "Button"];
                if (button is ToggleButton toggleButton)
                {
                    toggleButton.Enabled = true;
                    if (Provider.defaulTypes.Contains(enumerator.Current))
                        toggleButton.SimulateClick();
                }
                else
                {
                    LogManager.Log("Type " + enumerator.Current.ToString() + "not found in UI, may be new?", LogManager.Level.Warning);
                }
            }

            Provider.TextOnlyEvents = false;
        }
    }
}
