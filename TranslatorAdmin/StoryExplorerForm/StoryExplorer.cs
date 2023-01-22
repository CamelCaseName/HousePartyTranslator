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
		private bool SettingsVisible = false;
		private bool inInitialization = true;

		public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, string StoryName, Form Parent, CancellationToken cancellation)
		{
			InitializeComponent();

			//indicate ownership
			parentName = Parent.Name;

			//change draw order for this windows from bottom to top to top to bottom to remove flickering
			//use double buffering for that
			DoubleBuffered = true;

			//get contextprovider
			Context = new ContextProvider(IsStory, AutoLoad, FileName, StoryName, cancellation);
			engine = new GraphingEngine(Context, this, NodeInfoLabel);

			this.StoryName = Context.StoryName;
			this.FileName = Context.FileName;

			Text = $"StoryExplorer - Loading";

			//add custom paint event handler to draw all nodes and edges
			Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);
			FormClosing += new FormClosingEventHandler(SaveNodes);

			ColoringDepth.Value = TranslatorApp.Properties.Settings.Default.ColoringDepth;
			IdealLength.Value = (decimal)TranslatorApp.Properties.Settings.Default.IdealLength;
		}

		internal GraphingEngine Grapher { get { return engine; } }

		public string ParentName { get { return parentName; } }

		public void Initialize(bool singleFile)
		{
			if (singleFile)
			{
				Text = $"StoryExplorer - waiting";
				if (!Context.ParseFile() || Context.GotCancelled)
				{
					Close();
				}
				Text = $"StoryExplorer - {FileName}";
			}
			else
			{
				Text = $"StoryExplorer - waiting";
				//parse story, and not get cancelled xD
				if (!Context.ParseAllFiles() || Context.GotCancelled)
				{
					Close();
				}
				Text = $"StoryExplorer - {StoryName}";
			}
			inInitialization = false;
			Context.Layout.Start();
		}

		private void SaveNodes(object? sender, FormClosingEventArgs? e)
		{
			Context.SaveNodes();
			TranslatorApp.Properties.Settings.Default.Save();
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
			Grapher.Context.Layout.Start();
		}

		private void Stop_Click(object sender, EventArgs e)
		{
			Grapher.Context.Layout.Stop();
		}

		//todo more values and settings to change
		private void IdealLength_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization) StoryExplorerConstants.IdealLength = IdealLength.Value > 1 && IdealLength.Value < IdealLength.Maximum ? (float)IdealLength.Value : StoryExplorerConstants.IdealLength;
		}

		private void ColoringDepth_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization) StoryExplorerConstants.ColoringDepth = ColoringDepth.Value > 1 && ColoringDepth.Value < ColoringDepth.Maximum ? (int)ColoringDepth.Value : StoryExplorerConstants.ColoringDepth;
		}

		private void MenuShowButton_Click(object sender, EventArgs e)
		{
			SettingsVisible = !SettingsVisible;
			SettingsBox.Visible = SettingsVisible;
			MenuShowButton.Text = SettingsVisible ? "Hide Menu" : "Show Menu";
			Invalidate();
		}
	}
}
