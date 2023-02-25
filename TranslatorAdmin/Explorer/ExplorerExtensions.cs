﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorDesktopApp.Explorer
{
    [SupportedOSPlatform("windows")]
    internal static class ExplorerExtensions
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