using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Translator
{

    static class App
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            Start();
        }

        /// <summary>
        /// Starts the main form
        /// </summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static void Start()
        {
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
        private static Assembly? OnResolveAssembly(object? sender, ResolveEventArgs? args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args?.Name?? "");

            string path = assemblyName.Name + ".dll";
            if (!assemblyName?.CultureInfo?.Equals(CultureInfo.InvariantCulture) ?? false)
            {
                path = $"{assemblyName?.CultureInfo}\\${path}";
            }

            using System.IO.Stream? stream = executingAssembly?.GetManifestResourceStream(path);
            if (stream == null)
                return null;

            byte[] assemblyRawBytes = new byte[stream.Length];
            _ = stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
            return Assembly.Load(assemblyRawBytes);
        }
    }
}
