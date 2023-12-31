using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Versioning;

namespace Translator.Explorer
{
    [SupportedOSPlatform("windows")]
    public static class ExplorerExtensions
    {
        public static void ToLowQuality(this Graphics graphics)
        {
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }
    }
}
