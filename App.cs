using System;
using System.Windows.Forms;
using HousePartyTranslator.Managers;
using System.Reflection;
using System.Globalization;

namespace HousePartyTranslator
{

    static class App
    {

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
            _ = new TranslationManager();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Fenster());
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
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            if (!assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                path = $"{assemblyName.CultureInfo}\\${path}";
            }

            using (var stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return null;

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
