using System.Text;

namespace Translator.Explorer.OpenCL;

public static class Kernels
{
    public static string NBodyKernel = Encoding.Default.GetString(Properties.Resources.NBody);
    public static string EdgeKernel = Encoding.Default.GetString(Properties.Resources.Edges);
}
