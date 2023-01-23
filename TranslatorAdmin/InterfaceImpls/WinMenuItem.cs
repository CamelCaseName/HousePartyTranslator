using Translator.UICompatibilityLayer;

namespace TranslatorApp.InterfaceImpls
{
	public class WinMenuItem : ToolStripMenuItem, IMenuItem
	{
		public WinMenuItem() { }
		public WinMenuItem(string title) { Text = title; }
		public WinMenuItem(string title, EventHandler eventHandler) { Click += eventHandler; Text = title; }

		public new event EventHandler Click { add => base.Click += value; remove => base.Click -= value; }

		public new string Text { get => base.Text; set => base.Text = value; }
	}
}
