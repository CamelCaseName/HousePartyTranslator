using System;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Desktop.InterfaceImpls;
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

            Core.UICompatibilityLayer.Settings.Initialize(new WinSettings());
            if (Settings.UpdateSettings)
            {
                Settings.Upgrade();
                Settings.UpdateSettings = false;
                Settings.Default.Save();
            }

            MainForm = new Fenster();
            Application.Run(MainForm);
        }
    }
}
