using Translator.Core.Helpers;
using Translator.Desktop.UI;
using Settings = Translator.Desktop.InterfaceImpls.WinSettings;

namespace Translator.Desktop
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

            if (Settings.WDefault.UpdateSettings)
            {
                Settings.WDefault.Upgrade();
                Settings.WDefault.UpdateSettings = false;
                Settings.WDefault.Save();
            }

            Core.UICompatibilityLayer.Settings.Initialize(Settings.WDefault);
            MainForm = new Fenster();
            Application.Run(MainForm);
        }
    }
}
