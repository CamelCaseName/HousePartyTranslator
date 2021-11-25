using System;
using System.Windows.Forms;
using HousePartyTranslator.Managers;

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
            _ = new TranslationManager();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Fenster());
        }
    }
}
