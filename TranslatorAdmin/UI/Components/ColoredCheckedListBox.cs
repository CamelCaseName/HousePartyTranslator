﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Translator.Desktop.Helpers;

namespace Translator.Desktop.UI.Components
{
    /// <summary>
    /// Borrowed from https://stackoverflow.com/questions/2130934/how-change-the-color-of-selecteditem-in-checkedlistbox-in-windowsforms
    /// Creates a coloured Rectangle in each element, depending on checked state.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ColoredCheckedListBox : CheckedListBox
    {
        /// <summary>
        /// List containing all indices that are part of the search result
        /// </summary>
        public List<int> SearchResults = new();

        /// <summary>
        /// list containing all indices that are duplicates fo the english string
        /// </summary>
        public ObservableCollection<string> SimilarStringsToEnglish = new();

        /// <summary>
        /// Use double buffering, removes flicker
        /// </summary>
        protected new bool DoubleBuffered = true;

        /// <summary>
        /// A Override for the default OnDrawItem which replaces the background with a coloured rectangle given a List of indices
        /// </summary>
        /// <param name="e">The DrawItemEventArgs which will be manipulated</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var e2 =
                new DrawItemEventArgs
                (
                    e.Graphics,
                    e.Font,
                    e.Bounds,
                    e.Index,
                    e.State,
                    e.ForeColor, //colour yellow if it is part of the search, else colour normally
                    e.Index > -1 ?
                        SearchResults.Contains(e.Index) ?
                            Color.DarkOrange :
                            CheckedIndices.Contains(e.Index) ?
                                Color.FromArgb(80, 130, 80) :
                                SimilarStringsToEnglish.Contains(Items[e.Index].ToString() ?? string.Empty) ?
                                    Color.FromArgb(130, 80, 130) :
                                    Color.FromArgb(130, 80, 80)
                            :
                            e.BackColor
                );
            base.OnDrawItem(e2);
        }

        /// <summary>
        /// Hack to get exactly the behaviour we want (https://stackoverflow.com/a/3897126) adapted and expanded.
        /// Only allows double clicks on the far left of the control, or only single clicks elsewhere.
        /// </summary>
        /// <param name="m"> The windows Message to be used</param>
        protected override void WndProc(ref Message m)
        {
            // Filter WM_LBUTTONDBLCLK and MW_LBUTTONDOWN
            if (m.Msg is not Winutils.WM_LBUTTONDBLCLK and not Winutils.WM_LBUTTONDOWN)
            {
                base.WndProc(ref m);
            }
            else if (m.Msg == Winutils.WM_LBUTTONDOWN) //our own handle
            {
                //get mouse cursor position from message https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-lbuttondown
                byte[] bytes = BitConverter.GetBytes(m.LParam.ToInt64());
                short x = BitConverter.ToInt16(bytes, 0);
                short y = BitConverter.ToInt16(bytes, 2);

                //create point from cursor pos. low word is x, high word is y
                var CursorPosition = new Point(x, y);

                //if a new item would be selected, we can do that. or if the cursor is on one of the check marks. but else we do nothing
                if (SelectedIndex != IndexFromPoint(CursorPosition) || x < 14)
                {
                    base.WndProc(ref m);
                }
            }
        }
    }
}