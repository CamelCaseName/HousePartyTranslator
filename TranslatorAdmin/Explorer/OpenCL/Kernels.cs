using System.Text;

namespace Translator.Explorer.OpenCL;

internal static class Kernels
{
	public static string NBodyKernel = Encoding.Default.GetString(TranslatorDesktopApp.Properties.Resources.NBody);
    public static string EdgeKernel = Encoding.Default.GetString(TranslatorDesktopApp.Properties.Resources.Edges);
}
