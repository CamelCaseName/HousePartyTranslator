using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Translator.Desktop.Helpers
{
    [SupportedOSPlatform("Windows")]
    public static partial class Winutils
    {
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_PAINT = 0x00f;
        public const int WM_ERASEBKGND = 0x00e;
        public const int WM_MOUSEMOVE = 0x200;

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ValidateRect(IntPtr hWnd, ref RECT lpRect);
    }
}
