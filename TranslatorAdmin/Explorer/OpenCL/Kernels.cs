using System.Text;

namespace Translator.Desktop.Explorer.OpenCL;

internal static class Kernels
{
    public static string NBodyKernel = Encoding.Default.GetString(Properties.Resources.NBody);
    public static string EdgeKernel = Encoding.Default.GetString(Properties.Resources.Edges);
}
