namespace Translator.Explorer.Window
{
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	public partial class StoryExplorer : Form
	{
		private readonly ContextProvider Context;
		private readonly GraphingEngine engine;
		private readonly Form parentForm;
		private readonly string parentName;
		public readonly string FileName;
		public readonly string StoryName;

		public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, string StoryName, Form Parent, CancellationToken cancellation)
		{
			InitializeComponent();

			//indicate ownership
			parentName = Parent.Name;
			parentForm = Parent;
			this.StoryName = StoryName;
			this.FileName = FileName;

			//change draw order for this windows from bottom to top to top to bottom to remove flickering
			//use double buffering for that
			DoubleBuffered = true;

			//get contextprovider
			Context = new ContextProvider(IsStory, AutoLoad, FileName, StoryName, cancellation);
			engine = new GraphingEngine(Context, this, NodeInfoLabel);

			Text = $"StoryExplorer - Laoding";

			//add custom paint event handler to draw all nodes and edges
			Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);
			FormClosing += new FormClosingEventHandler(SaveNodes);

		}

		public Form FormParent { get { return parentForm; } }

		internal GraphingEngine Grapher { get { return engine; } }

		public string ParentName { get { return parentName; } }

		public void Initialize(bool singleFile)
		{
			if (singleFile)
			{
				Text = $"StoryExplorer - {FileName}";
				if (!Context.ParseFile() || Context.GotCancelled)
				{
					Close();
				}
			}
			else
			{
				Text = $"StoryExplorer - {StoryName}";
				//parse story, and not get cancelled xD
				if (!Context.ParseAllFiles() || Context.GotCancelled)
				{
					Close();
				}
			}
			Context.StartLayoutCalculations();
		}

		private void SaveNodes(object? sender, FormClosingEventArgs? e)
		{
			Context.SaveNodes();
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
