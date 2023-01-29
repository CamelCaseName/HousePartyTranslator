﻿using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.Versioning;
using System.Text;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;

namespace TranslatorApp.InterfaceImpls
{
	[SupportedOSPlatform("windows")]
	public class WinTextBox : TextBox, ITextBox
	{
		private const int WM_PAINT = 15;
		private const int WM_MOUSEMOVE = 512;
		private const int WM_LBUTTONDOWN = 513;
		private const int WM_RBUTTONDOWN = 516;
		private const int WM_MBUTTONDOWN = 519;
		private bool customDrawNeeded = false;
		public WinTextBox() : base()
		{
			MouseDown += (object? sender, MouseEventArgs e) => { customDrawNeeded = true; Invalidate(); };
			MouseUp += (object? sender, MouseEventArgs e) => { customDrawNeeded = true; Invalidate(); };
			MouseDoubleClick += (object? sender, MouseEventArgs e) => { customDrawNeeded = true; Invalidate(); };
			GotFocus += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
			Click += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
			MouseCaptureChanged += (object? sender, EventArgs e) => { customDrawNeeded = true; Invalidate(); };
			MouseMove += (object? sender, MouseEventArgs e) => _ = e.Button == MouseButtons.Left ? customDrawNeeded = true : customDrawNeeded = false;
		}
		public int SelectionEnd
		{
			get => base.SelectionStart + base.SelectionLength;
			set { if (SelectionStart <= SelectionEnd) base.SelectionLength = value - SelectionStart; else throw new ArgumentOutOfRangeException(nameof(SelectionEnd), "End has to be after SelectionStart"); }
		}
		public new int SelectionStart { get => base.SelectionStart; set => base.SelectionStart = value; }
		public int HighlightStart { get; set; }
		public int HighlightEnd { get; set; }
		private bool showHighlight = false;
		public bool ShowHighlight { get => showHighlight; set { Invalidate(); showHighlight = value; customDrawNeeded = true; } }
		public new string Text { get => base.Text; set { Invalidate(); customDrawNeeded = true;  base.Text = value; } }

		public new void Focus() => base.Focus();

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			
			//if we have a WM_PAINT and should display a shighlight somewhere
			if (customDrawNeeded  && ShowHighlight && HighlightEnd > HighlightStart && HighlightStart < Text.Length && HighlightEnd < Text.Length)
			{
				//overlay the other text highlight
				//get text positions
				//todo split draw calls over multiple so sentences changing line are preserved
				Point highlightLocation = GetPositionFromCharIndex(HighlightStart);
				highlightLocation.X -= 2;
				TextRenderer.DrawText(e.Graphics, Text.AsSpan()[HighlightStart..HighlightEnd], base.Font, highlightLocation, Utils.darkText, Utils.highlight);
				customDrawNeeded = false;
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if ((m.Msg == WM_PAINT || m.Msg == WM_MOUSEMOVE) && IsHandleCreated && customDrawNeeded)
				//we have a paint message, send to own handler. only if we have a gdi handle
				OnPaint(new PaintEventArgs(Graphics.FromHwnd(m.HWnd), ClientRectangle));
		}
	}
}
