using System;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TranslationManager tManager = new TranslationManager();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Fenster());
        }
    }
}
