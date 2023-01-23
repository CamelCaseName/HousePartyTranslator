using System.Globalization;
using System.Reflection;
using Settings = TranslatorApp.InterfaceImpls.WinSettings;

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
			AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
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

		/// <summary>
		/// Hooks to assembly resolver and tries to load assembly (.dll)
		/// from executable resources it CLR can't find it locally.
		///
		/// Used for embedding assemblies onto executables.
		///
		/// See: http://www.digitallycreated.net/Blog/61/combining-multiple-assemblies-into-a-single-exe-for-a-wpf-application
		/// borrowed from https://gist.github.com/x1unix/7bced85295bb3fbc21a7308bf541e2b8
		/// </summary>
		private static Assembly? OnResolveAssembly(object? sender, ResolveEventArgs? args)
		{
			var executingAssembly = Assembly.GetExecutingAssembly();
			var assemblyName = new AssemblyName(args?.Name ?? "");

			string path = assemblyName.Name + ".dll";
			if (!assemblyName?.CultureInfo?.Equals(CultureInfo.InvariantCulture) ?? false)
			{
				path = $"{assemblyName?.CultureInfo}\\${path}";
			}

			using Stream? stream = executingAssembly?.GetManifestResourceStream(path);
			if (stream == null)
				return null;

			byte[] assemblyRawBytes = new byte[stream.Length];
			_ = stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
			return Assembly.Load(assemblyRawBytes);
		}
	}
}
