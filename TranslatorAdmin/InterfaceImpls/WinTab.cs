using System.Runtime.Versioning;
using Translator.Core.Helpers;
using Translator.Helpers;
using Translator.UICompatibilityLayer;

namespace Translator.InterfaceImpls
{
	[SupportedOSPlatform("Windows")]
	public class WinTab : TabPage, ITab<WinLineItem>
	{
		//todo add progress bar to tab title
		public WinTab() { MainForm = (Fenster)new Form(); }

		internal Fenster MainForm { get; init; }

		private static int Number { get; set; } = 0;

		private readonly Button TranslateThis = new();
		private readonly CheckBox ApprovedBox = new();
		private readonly ContextMenuStrip ListContextMenu = new();
		private readonly GroupBox CommentGroup = new();
		private readonly Label CharacterCountLabel = new();
		private readonly Label LinesTranslated = new();
		private readonly Label SelectedFile = new();
		private readonly LineList CheckListBoxLeft = new();
		private readonly Panel panel1 = new();
		private readonly Panel panel2 = new();
		private readonly TableLayoutPanel mainTableLayoutPanel = new();
		private readonly WinMenuItem CopyAllContextMenuButton = new();
		private readonly WinMenuItem CopyAsOutputContextMenuButton = new();
		private readonly WinMenuItem CopyFileNameContextMenuButton = new();
		private readonly WinMenuItem CopyIdContextMenuButton = new();
		private readonly WinMenuItem CopyStoryNameContextMenuButton = new();
		private readonly WinMenuItem CopyTemplateContextMenuButton = new();
		private readonly WinMenuItem CopyTranslationContextMenuButton = new();
		private readonly WinTextBox CommentTextBox = new();
		private readonly WinTextBox TemplateTextBox = new();
		private readonly WinTextBox TranslationTextBox = new();
		public readonly NoAnimationBar ProgressbarTranslated = new();

		public WinTab(Fenster fenster)
		{
			MainForm = fenster;
			++Number;
			BackColor = Color.Black;
			ForeColor = SystemColors.ScrollBar;
			BorderStyle = BorderStyle.FixedSingle;
			Location = new Point(4, 22);
			Name = $"TabPage{Number}";
			Padding = new Padding(3);
			TabIndex = 0;
			Text = $"Tab{Number}";
			mainTableLayoutPanel.SuspendLayout();
			CommentGroup.SuspendLayout();
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			SuspendLayout();
			// 
			// TranslationTextBox
			// 
			TranslationTextBox.AcceptsReturn = true;
			TranslationTextBox.AllowDrop = true;
			TranslationTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
			TranslationTextBox.BackColor = Utils.background;
			TranslationTextBox.Dock = DockStyle.Fill;
			TranslationTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
			TranslationTextBox.ForeColor = Utils.brightText;
			TranslationTextBox.ImeMode = ImeMode.On;
			TranslationTextBox.Location = new Point(689, 294);
			TranslationTextBox.Multiline = true;
			TranslationTextBox.Name = "TranslationTextBox";
			TranslationTextBox.Size = new Size(678, 275);
			TranslationTextBox.TabIndex = 0;
			TranslationTextBox.TextChanged += new EventHandler(MainForm.TextBoxRight_TextChanged);
			TranslationTextBox.MouseUp += new MouseEventHandler(MainForm.TextContextOpened);
			TranslationTextBox.MouseEnter += new EventHandler(MainForm.TextContextOpened);
			TranslationTextBox.PlaceholderText = "Translation goes here";
			// 
			// AutoTranslateThis
			// 
			TranslateThis.AutoSize = true;
			TranslateThis.BackColor = Utils.menu;
			TranslateThis.ForeColor = Utils.darkText;
			TranslateThis.Location = new Point(80, 1);
			TranslateThis.Name = "AutoTranslateThis";
			TranslateThis.Size = new Size(60, 20);
			TranslateThis.TabIndex = 13;
			TranslateThis.Text = "Automatic Translation";
			TranslateThis.UseVisualStyleBackColor = true;
			TranslateThis.Click += new EventHandler(MainForm.TranslateThis_Click);
			// 
			// TemplateTextBox
			// 
			TemplateTextBox.BackColor = Utils.background;
			TemplateTextBox.Dock = DockStyle.Fill;
			TemplateTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
			TemplateTextBox.ForeColor = Utils.brightText;
			TemplateTextBox.Location = new Point(689, 33);
			TemplateTextBox.Multiline = true;
			TemplateTextBox.Name = "TemplateTextBox";
			TemplateTextBox.ReadOnly = true;
			TemplateTextBox.Size = new Size(678, 255);
			TemplateTextBox.TabIndex = 9;
			TemplateTextBox.PlaceholderText = "Template will be here";
			// 
			// CommentTextBox
			// 
			CommentTextBox.BackColor = Utils.background;
			CommentTextBox.Dock = DockStyle.Fill;
			CommentTextBox.Font = new Font("Consolas", 11F);
			CommentTextBox.ForeColor = Utils.brightText;
			CommentTextBox.Location = new Point(3, 16);
			CommentTextBox.Multiline = true;
			CommentTextBox.Name = "CommentTextBox";
			CommentTextBox.Size = new Size(672, 105);
			CommentTextBox.TabIndex = 13;
			CommentTextBox.TextChanged += new EventHandler(MainForm.Comments_TextChanged);
			CommentTextBox.MouseUp += new MouseEventHandler(MainForm.TextContextOpened);
			CommentTextBox.MouseEnter += new EventHandler(MainForm.TextContextOpened);
			CommentTextBox.PlaceholderText = "No comments yet";
			// 
			// CharacterCountLabel
			// 
			CharacterCountLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			CharacterCountLabel.AutoSize = true;
			CharacterCountLabel.ForeColor = Utils.brightText;
			CharacterCountLabel.Location = new Point(23, 5);
			CharacterCountLabel.Name = "CharacterCountLabel";
			CharacterCountLabel.Size = new Size(143, 13);
			CharacterCountLabel.TabIndex = 20;
			CharacterCountLabel.Text = "Template: xx | Translation: xx";
			// 
			// SelectedFile
			// 
			SelectedFile.AutoSize = true;
			SelectedFile.ForeColor = Utils.brightText;
			SelectedFile.Location = new Point(0, 6);
			SelectedFile.Name = "SelectedFile";
			SelectedFile.Size = new Size(98, 13);
			SelectedFile.TabIndex = 7;
			SelectedFile.Text = "Selected File: none";
			// 
			// LinesTranslated
			// 
			LinesTranslated.Anchor = AnchorStyles.Top;
			LinesTranslated.Parent = panel1;
			LinesTranslated.AutoSize = true;
			LinesTranslated.BackColor = Color.Transparent;
			LinesTranslated.ForeColor = Utils.brightText;
			LinesTranslated.Location = new Point(60, 6);
			LinesTranslated.Name = "LinesTranslated";
			LinesTranslated.Size = new Size(47, 13);
			LinesTranslated.TabIndex = 15;
			LinesTranslated.Text = "progress";
			// 
			// ApprovedBox
			// 
			ApprovedBox.AutoSize = true;
			ApprovedBox.ForeColor = Utils.brightText;
			ApprovedBox.Location = new Point(3, 5);
			ApprovedBox.Name = "ApprovedBox";
			ApprovedBox.Size = new Size(72, 17);
			ApprovedBox.TabIndex = 13;
			ApprovedBox.Text = "Approved";
			ApprovedBox.UseVisualStyleBackColor = true;
			ApprovedBox.CheckedChanged += new EventHandler(MainForm.ApprovedBox_CheckedChanged);
			// 
			// mainTableLayoutPanel
			// 
			mainTableLayoutPanel.ColumnCount = 2;
			_ = mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.07924F));
			_ = mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.92076F));
			mainTableLayoutPanel.Controls.Add(CommentGroup, 1, 3);
			mainTableLayoutPanel.Controls.Add(TranslationTextBox, 1, 2);
			mainTableLayoutPanel.Controls.Add(TemplateTextBox, 1, 1);
			mainTableLayoutPanel.Controls.Add(CheckListBoxLeft, 0, 1);
			mainTableLayoutPanel.Controls.Add(panel1, 0, 0);
			mainTableLayoutPanel.Controls.Add(panel2, 1, 0);
			mainTableLayoutPanel.Parent = this;
			mainTableLayoutPanel.Dock = DockStyle.Fill;
			mainTableLayoutPanel.Location = new Point(3, 3);
			mainTableLayoutPanel.Name = "mainTableLayoutPanel";
			mainTableLayoutPanel.RowCount = 4;
			_ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
			_ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 38.94275F));
			_ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 41.86569F));
			_ = mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 19.19156F));
			mainTableLayoutPanel.Size = new Size(1370, 702);
			mainTableLayoutPanel.TabIndex = 18;
			// 
			// CommentGroup
			// 
			CommentGroup.Controls.Add(CommentTextBox);
			CommentGroup.Dock = DockStyle.Fill;
			CommentGroup.ForeColor = Utils.brightText;
			CommentGroup.Location = new Point(689, 575);
			CommentGroup.Name = "CommentGroup";
			CommentGroup.Size = new Size(678, 124);
			CommentGroup.TabIndex = 11;
			CommentGroup.TabStop = false;
			CommentGroup.Text = "Comments";
			// 
			// panel1
			// 
			panel1.Controls.Add(SelectedFile);
			panel1.Controls.Add(LinesTranslated);
			panel1.Controls.Add(ProgressbarTranslated);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(3, 3);
			panel1.Name = "panel1";
			panel1.Size = new Size(680, 24);
			panel1.TabIndex = 12;
			// 
			// panel2
			// 
			panel2.Controls.Add(ApprovedBox);
			panel2.Controls.Add(TranslateThis);
			panel2.Controls.Add(CharacterCountLabel);
			panel2.Dock = DockStyle.Fill;
			panel2.Location = new Point(689, 3);
			panel2.Name = "panel2";
			panel2.Size = new Size(678, 24);
			panel2.TabIndex = 13;
			// 
			// CheckListBoxLeft
			// 
			CheckListBoxLeft.BackColor = Utils.frame;
			CheckListBoxLeft.Dock = DockStyle.Fill;
			CheckListBoxLeft.ForeColor = Utils.brightText;
			CheckListBoxLeft.FormattingEnabled = true;
			CheckListBoxLeft.Location = new Point(3, 33);
			CheckListBoxLeft.Name = "CheckListBoxLeft";
			mainTableLayoutPanel.SetRowSpan(CheckListBoxLeft, 3);
			CheckListBoxLeft.Size = new Size(680, 666);
			CheckListBoxLeft.TabIndex = 10;
			CheckListBoxLeft.ThreeDCheckBoxes = true;
			CheckListBoxLeft.ItemCheck += new ItemCheckEventHandler(MainForm.CheckListBoxLeft_ItemCheck);
			CheckListBoxLeft.SelectedIndexChanged += new EventHandler(MainForm.CheckListBoxLeft_SelectedIndexChanged);
			CheckListBoxLeft.ContextMenuStrip = ListContextMenu;
			CheckListBoxLeft.MouseDown += new MouseEventHandler(MainForm.OpeningContextMenu);
			// 
			// ProgressbarTranslated
			// 
			ProgressbarTranslated.BackColor = Utils.background;
			ProgressbarTranslated.Cursor = Cursors.Default;
			ProgressbarTranslated.Dock = DockStyle.Fill;
			ProgressbarTranslated.ForeColor = Utils.foreground;
			ProgressbarTranslated.Location = new Point(0, 0);
			ProgressbarTranslated.Name = "ProgressbarTranslated";
			ProgressbarTranslated.Size = new Size(680, 24);
			ProgressbarTranslated.Step = 1;
			ProgressbarTranslated.Style = ProgressBarStyle.Continuous;
			ProgressbarTranslated.TabIndex = 8;
			ProgressbarTranslated.Value = 50;
			// 
			// CopyIdContextMenuButton
			// 
			CopyIdContextMenuButton.Name = "CopyIdContextMenuButton";
			CopyIdContextMenuButton.Size = new Size(236, 22);
			CopyIdContextMenuButton.Text = "Copy Id";
			CopyIdContextMenuButton.Click += new EventHandler(MainForm.CopyIdContextMenuButton_Click);
			// 
			// CopyFileNameContextMenuButton
			// 
			CopyFileNameContextMenuButton.Name = "CopyFileNameContextMenuButton";
			CopyFileNameContextMenuButton.Size = new Size(236, 22);
			CopyFileNameContextMenuButton.Text = "Copy file name";
			CopyFileNameContextMenuButton.Click += new EventHandler(MainForm.CopyFileNameContextMenuButton_Click);
			// 
			// CopyStoryNameContextMenuButton
			// 
			CopyStoryNameContextMenuButton.Name = "CopyStoryNameContextMenuButton";
			CopyStoryNameContextMenuButton.Size = new Size(236, 22);
			CopyStoryNameContextMenuButton.Text = "Copy story name";
			CopyStoryNameContextMenuButton.Click += new EventHandler(MainForm.CopyStoryNameContextMenuButton_Click);
			// 
			// CopyAllContextMenuButton
			// 
			CopyAllContextMenuButton.Name = "CopyAllContextMenuButton";
			CopyAllContextMenuButton.Size = new Size(236, 22);
			CopyAllContextMenuButton.Text = "Copy everything";
			CopyAllContextMenuButton.Click += new EventHandler(MainForm.CopyAllContextMenuButton_Click);
			// 
			// CopyAsOutputContextMenuButton
			// 
			CopyAsOutputContextMenuButton.Name = "CopyAsOutputContextMenuButton";
			CopyAsOutputContextMenuButton.Size = new Size(236, 22);
			CopyAsOutputContextMenuButton.Text = "Copy as output";
			CopyAsOutputContextMenuButton.Click += new EventHandler(MainForm.CopyAsOutputContextMenuButton_Click);
			// 
			// CopyTemplateContextMenuButton
			// 
			CopyTemplateContextMenuButton.Name = "CopyTemplateContextMenuButton";
			CopyTemplateContextMenuButton.Size = new Size(236, 22);
			CopyTemplateContextMenuButton.Text = "Copy template";
			CopyTemplateContextMenuButton.Click += new EventHandler(MainForm.CopyTemplateContextMenuButton_Click);
			// 
			// CopyTranslationContextMenuButton
			// 
			CopyTranslationContextMenuButton.Name = "CopyTranslationContextMenuButton";
			CopyTranslationContextMenuButton.Size = new Size(236, 22);
			CopyTranslationContextMenuButton.Text = "Copy translation";
			CopyTranslationContextMenuButton.Click += new EventHandler(MainForm.CopyTranslationContextMenuButton_Click);
			//
			// ListContextMenu
			//
			ListContextMenu.Name = "ListContextMenu";
			ListContextMenu.BackColor = SystemColors.ScrollBar;
			ListContextMenu.ShowCheckMargin = false;
			ListContextMenu.ShowImageMargin = false;
			ListContextMenu.ForeColor = SystemColors.MenuText;
			ListContextMenu.Size = new Size(236, 160);
			ListContextMenu.Items.Clear();
			ListContextMenu.Items.AddRange(new ToolStripItem[]{
				CopyIdContextMenuButton,
				CopyFileNameContextMenuButton,
				CopyStoryNameContextMenuButton,
				CopyTemplateContextMenuButton,
				CopyTranslationContextMenuButton,
				CopyAsOutputContextMenuButton,
				CopyAllContextMenuButton
			});

			Controls.Add(mainTableLayoutPanel);

			mainTableLayoutPanel.ResumeLayout();
			mainTableLayoutPanel.PerformLayout();
			CommentGroup.ResumeLayout();
			CommentGroup.PerformLayout();
			panel1.ResumeLayout();
			panel1.PerformLayout();
			panel2.ResumeLayout();
			panel2.PerformLayout();
			ResumeLayout();
		}

		public bool IsApproveButtonFocused => ApprovedBox.Focused;

		public List<string> SimilarStringsToEnglish => Lines.SimilarStringsToEnglish;

		public int SelectedLineIndex => Lines.SelectedIndex;

		public WinLineItem SelectedLineItem => (WinLineItem?)Lines.SelectedItem ?? new WinLineItem();

		public bool IsTranslationBoxFocused => TranslationTextBox.Focused;

		public bool IsCommentBoxFocused => CommentTextBox.Focused;

		public int SingleProgressValue { get => ProgressbarTranslated.Value; set => ProgressbarTranslated.Value = value; }
        public int AllProgressValue { get => ProgressbarTranslated.Value; set => ProgressbarTranslated.Value = value; }
        public string TranslationBoxText { get => TranslationTextBox.Text; set => TranslationTextBox.Text = value; }

		public string SelectedTranslationBoxText => TranslationTextBox.SelectedText;

		public string TemplateBoxText { get => TemplateTextBox.Text; set => TemplateTextBox.Text = value; }

		public string SelectedTemplateBoxText => TemplateTextBox.SelectedText;

		public string CommentBoxText { get => CommentTextBox.Text; set => CommentTextBox.Text = value; }
		public string[] CommentBoxTextArr { get => CommentTextBox.Lines; set => CommentTextBox.Lines = value; }
		public bool ApprovedButtonChecked { get => ApprovedBox.Checked; set => ApprovedBox.Checked = value; }
		public LineList Lines { get => CheckListBoxLeft; }
		ILineList<WinLineItem> ITab<WinLineItem>.Lines
		{
			get => Lines;
			set
			{
				Lines.Clear(); for (int i = 0; i < value.Count; i++)
				{
					Lines.Add(value[i].Text, value[i].IsApproved);
				}
			}
		}

		public int LineCount { get => Lines.Count; }

		string ITab<WinLineItem>.Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				Update();
			}
		}

		public ITextBox Translation => TranslationTextBox;

		public ITextBox Template => TemplateTextBox;

		public ITextBox Comments => CommentTextBox;

		public void ApproveSelectedLine() => CheckListBoxLeft.SetItemChecked(SelectedLineIndex, true);
		public WinLineItem AtIndex(int index) => CheckListBoxLeft[index];
		public void ClearLines() => CheckListBoxLeft.Clear();
		public void FocusCommentBox() => CommentTextBox.Focus();
		public void FocusTranslationBox() => TranslationTextBox.Focus();
		public string SelectedCommentBoxText() => CommentTextBox.SelectedText;
		public void SelectLineItem(int index) => CheckListBoxLeft.SelectedIndex = index;
		public void SelectLineItem(WinLineItem item) => CheckListBoxLeft.SelectedItem = item;
		public void SetApprovedLabelText(string text) => LinesTranslated.Text = text;
		public void SetCharacterCountLabelText(string text) => CharacterCountLabel.Text = text;
		public void SetCharacterLabelColor(Color color) => CharacterCountLabel.ForeColor = color;
		public void SetFileInfoText(string info) => SelectedFile.Text = info;
		public void SetSelectedCommentBoxText(int start, int end) { CommentTextBox.SelectionStart = start; CommentTextBox.SelectionEnd = end; }
		public void SetSelectedTemplateBoxText(int start, int end) { TemplateTextBox.SelectionStart = start; TemplateTextBox.SelectionEnd = end; }
		public void SetSelectedTranslationBoxText(int start, int end) { TranslationTextBox.SelectionStart = start; TranslationTextBox.SelectionEnd = end; }
		public void UnapproveSelectedLine() => CheckListBoxLeft.SetItemChecked(SelectedLineIndex, false);
		public void UpdateLines() => CheckListBoxLeft.Update();
	}
}
