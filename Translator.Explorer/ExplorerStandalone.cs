using Translator.Core.UICompatibilityLayer;
using Translator.Desktop.InterfaceImpls;
using Translator.Explorer.Window;
using Translator.Helpers;

namespace Translator.Explorer
{
    public static class ExplorerStandalone
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Settings.Initialize(WinSettings.Default);
            DialogResult openAll = Msg.InfoYesNoCancel(
                       $"Do you want to explore all files for a story or a single file?\nNote: more files means slower layout, but viewing performance is about the same.",
                       "All files?"
                       );

            if (openAll == DialogResult.Cancel)
                return;

            var explorer = new StoryExplorer(false, true, "", "", new CancellationToken());
            explorer.Shown += (_, _) => explorer.Initialize(openAll == DialogResult.No);
            Application.Run(explorer);
        }
    }
}