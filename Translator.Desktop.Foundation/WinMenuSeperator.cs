﻿using System;
using System.Windows.Forms;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Desktop.UI.Components
{
    public class WinMenuSeperator : ToolStripSeparator, IMenuItem
    {
        public new event EventHandler Click { add => base.Click += value; remove => base.Click -= value; }
        public new string Text { get => base.Text; set => base.Text = value; }
    }
}
