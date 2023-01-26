﻿namespace Translator.Explorer.Window
{
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	public partial class StoryExplorer : Form
	{
		private readonly ContextProvider Context;
		private readonly GraphingEngine engine;
		private readonly NodeProvider provider;
		private readonly string parentName;
		public readonly string FileName;
		public readonly string StoryName;
		private bool SettingsVisible = false;
		private bool inInitialization = true;
		public const string Version = "1.0.0.0";
		public const string Title = "StoryExplorer v" + Version;

		public StoryExplorer(bool IsStory, bool AutoLoad, string FileName, string StoryName, Form Parent, CancellationToken cancellation)
		{
			InitializeComponent();

			//indicate ownership
			parentName = Parent.Name;

			//change draw order for this windows from bottom to top to top to bottom to remove flickering
			//use double buffering for that
			DoubleBuffered = true;

			//get contextprovider
			provider = new();
			Context = new(provider, IsStory, AutoLoad, FileName, StoryName, cancellation);
			engine = new(provider, this, NodeInfoLabel);

			this.StoryName = Context.StoryName;
			this.FileName = Context.FileName;

			Text = Title + " - Loading";

			//add custom paint event handler to draw all nodes and edges
			Paint += new PaintEventHandler(Grapher.DrawNodesPaintHandler);
			FormClosing += new FormClosingEventHandler(SaveNodes);

			ColoringDepth.Value = StoryExplorerConstants.ColoringDepth = TranslatorApp.Properties.Settings.Default.ColoringDepth;
			IdealLength.Value = (decimal)(StoryExplorerConstants.IdealLength = TranslatorApp.Properties.Settings.Default.IdealLength);
			NodeSizeField.Value = StoryExplorerConstants.Nodesize;
		}

		internal GraphingEngine Grapher { get { return engine; } }
		internal NodeProvider Provider{ get { return provider; } }

		public string ParentName { get { return parentName; } }

		public void Initialize(bool singleFile)
		{
			if (singleFile)
			{
				Text = Title + " - waiting";
				if (!Context.ParseFile() || Context.GotCancelled)
				{
					Close();
				}
				Text = Title + $" - {FileName}";
			}
			else
			{
				Text = Title + " - waiting";
				//parse story, and not get cancelled xD
				if (!Context.ParseAllFiles() || Context.GotCancelled)
				{
					Close();
				}
				Text = Title + $" - {StoryName}";
			}
			inInitialization = false;
			NodeCalculations.Text = "Calculation running";

			provider.FreezeNodesAsInitial();
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
			NodeCalculations.Text = "Calculation running";
			Context.Layout.Start();
			NodeCalculations.Update();
			NodeCalculations.Invalidate();
		}

		private void Stop_Click(object sender, EventArgs e)
		{
			NodeCalculations.Text = "Calculation stopped";
			Context.Layout.Stop();
			NodeCalculations.Update();
			NodeCalculations.Invalidate();
		}

		//todo more values and settings to change
		//- base node colors?
		//- single node colors
		//- target fps?
		//todo add more info, maybe as list of values, treelist under info text
		//todo add internal nodes visible button and setting
		//todo add setting to allow only a set number/type of nodes
		private void IdealLength_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization)
			{
				StoryExplorerConstants.IdealLength = IdealLength.Value > 1 && IdealLength.Value < IdealLength.Maximum ? (float)IdealLength.Value : StoryExplorerConstants.IdealLength;
				TranslatorApp.Properties.Settings.Default.IdealLength = (float)IdealLength.Value;
			}

		}

		private void ColoringDepth_ValueChanged(object sender, EventArgs e)
		{
			if (!inInitialization)
			{
				StoryExplorerConstants.ColoringDepth = ColoringDepth.Value > 1 && ColoringDepth.Value < ColoringDepth.Maximum ? (int)ColoringDepth.Value : StoryExplorerConstants.ColoringDepth;
				TranslatorApp.Properties.Settings.Default.ColoringDepth = (int)ColoringDepth.Value;
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