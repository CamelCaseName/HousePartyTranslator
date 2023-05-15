using System.Text;

namespace Translator.Desktop.Explorer.OpenCL;

internal static class Kernels
{
    public static string NBodyKernel = Encoding.Default.GetString(Desktop.Properties.Resources.NBody);
    public static string EdgeKernel = Encoding.Default.GetString(Desktop.Properties.Resources.Edges);
}
