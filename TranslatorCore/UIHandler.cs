using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator.Core
{
    /// <summary>
    /// Provides a clean Interface for managing the UI, without having to use the respective ui elements by themselves. ui platform independant in some way.
    /// </summary>
    public static class UIHandler
    {
        /// <summary>
        /// Signals the user that the application is working and the might need to wait
        /// </summary>
        public static void SignalAppWait()
        {
            Application.UseWaitCursor= true;
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        public static void SignalAppExit()
        {
            Application.Exit();
        }

        /// <summary>
        /// Signals the user that the wait is over.
        /// </summary>
        public static void SignalAppUnWait()
        {
            Application.UseWaitCursor= false;
        }
    }
}
