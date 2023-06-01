using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator.Desktop.InterfaceImpls;

namespace Translator.Desktop.UI.Components
{
    internal sealed class SearchToolStripTextBox : WinToolStripTextBox
    {
        public int TotalSearchResults
        {
            get
            {
                return _totalSearchResults;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(TotalSearchResults),
                        $"{nameof(TotalSearchResults)} cannot be negative");
                }
                else
                {
                    _totalSearchResults = value;
                }
            }
        }
        private int _totalSearchResults = 0;

        public int CurrentSearchResults
        {
            get
            {
                return _currentSearchResults;
            }

            set
            {
                if (value < 0 && value >= TotalSearchResults)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(CurrentSearchResults),
                        $"{nameof(CurrentSearchResults)} cannot be negative or greater than the total number of results");
                }
                else
                {
                    _currentSearchResults = value;
                }
            }
        }
        private int _currentSearchResults = 0;
    }
}
