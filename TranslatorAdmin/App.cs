using System.Globalization;
using System.Reflection;
using Translator.Core;
using Settings = Translator.InterfaceImpls.WinSettings;

namespace Translator
{

	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	public static class App
	{
#nullable disable
		public static Fenster MainForm { get; private set; }
#nullable enable
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
			LogManager.Log("App started.");
			Start();
		}

		/// <summary>
		/// Starts the main form
		/// </summary>
		private static void Start()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (((Settings)Settings.Default).UpdateSettings)
			{
				((Settings)Settings.Default).Upgrade();
				((Settings)Settings.Default).UpdateSettings = false;
				Settings.Default.Save();
			}

			MainForm = new Fenster();
			Application.Run(MainForm);
		}
	}
}
