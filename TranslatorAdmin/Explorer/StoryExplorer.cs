using System.Runtime.InteropServices;

namespace Translator.Explorer.Window
{
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	internal partial class StoryExplorer : Form
	{
		private readonly ContextProvider Context;
		private readonly GraphingEngine engine;
		private readonly string parentName;
		public readonly string FileName;
		public readonly string StoryName;
		private bool SettingsVisible = false;
		private bool inInitialization = true;
		public const string Version = "1.0.1.0";
		public const string Title = "StoryExplorer v" + Version;
		private readonly CancellationToken token;
		public NodeLayout? Layouter { get; private set; }
		internal GraphingEngine Grapher { get { return engine; } }
		internal NodeProvider Provider { get; }
		public string ParentName { get { return parentName; } }

		public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, string StoryName, Form Parent, CancellationToken cancellation)
		{
			InitializeComponent();
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

			this.StoryName = Context.StoryName;
			this.FileName = Context.FileName;

			Text = Title + " - Loading";

			//add custom paint event handler to draw all nodes and edges
			Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);
			FormClosing += new FormClosingEventHandler(SaveNodes);

			ColoringDepth.Value = StoryExplorerConstants.ColoringDepth = Properties.Settings.Default.ColoringDepth;
			IdealLength.Value = (decimal)(StoryExplorerConstants.IdealLength = Properties.Settings.Default.IdealLength);
			NodeSizeField.Value = StoryExplorerConstants.Nodesize;
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
			Layouter.Start();
			Invalidate();
		}

		public void SaveNodes()
		{
			Layouter?.Stop();
			_ = Context.SaveNodes(Provider.Nodes);
		}

		private void SaveNodes(object? sender, FormClosingEventArgs? e)
		{
			_ = Context.SaveNodes(Provider.Nodes);
			Properties.Settings.Default.Save();
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
			NodeCalculations.Text = "Calculation running";
			Layouter?.Start();
			NodeCalculations.Update();
			NodeCalculations.Invalidate();
		}

		public void Stop_Click(object sender, EventArgs e)
		{
			NodeCalculations.Text = "Calculation stopped";
			if(Layouter?.Started ?? false)Layouter?.Stop();
			NodeCalculations.Update();
			NodeCalculations.Invalidate();
		}

		//todo more values and settings to change
		//- base node colors?
		//- single node colors
		//- all other node colors
		//- target fps?
		//todo add more info, maybe as list of values, treelist under info text
		//todo add internal nodes visible button and setting
		//todo add setting to allow only a set number/type of nodes
		private void IdealLength_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization)
			{
				StoryExplorerConstants.IdealLength = IdealLength.Value > 1 && IdealLength.Value < IdealLength.Maximum ? (float)IdealLength.Value : StoryExplorerConstants.IdealLength;
				Properties.Settings.Default.IdealLength = (float)IdealLength.Value;
			}

		}

		private void ColoringDepth_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization)
			{
				StoryExplorerConstants.ColoringDepth = ColoringDepth.Value > 1 && ColoringDepth.Value < ColoringDepth.Maximum ? (int)ColoringDepth.Value : StoryExplorerConstants.ColoringDepth;
				Properties.Settings.Default.ColoringDepth = (int)ColoringDepth.Value;
			}
		}

		private void MenuShowButton_Click(object sender, EventArgs e)
		{
			SettingsVisible = !SettingsVisible;
			SettingsBox.Visible = SettingsVisible;
			MenuShowButton.Text = SettingsVisible ? "Hide Menu" : "Show Menu";
			SettingsBox.Update();
			MenuShowButton.Update();
			SettingsBox.Invalidate();
			MenuShowButton.Invalidate();
		}

		private void NodeSizeField_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization)
			{
				StoryExplorerConstants.Nodesize = NodeSizeField.Value > 1 && NodeSizeField.Value < NodeSizeField.Maximum ? (int)NodeSizeField.Value : StoryExplorerConstants.Nodesize;
			}
		}
	}
}
