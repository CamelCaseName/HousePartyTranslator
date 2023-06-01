using System;

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
                        $"{nameof(TotalSearchResults)} must not be negative");
                }
                else
                {
                    _totalSearchResults = value;
                    _counter = string.Concat(_currentSearchResult.ToString(), "/", _totalSearchResults.ToString());
                }
            }
        }
        private int _totalSearchResults = 0;

        public int CurrentSearchResult
        {
            get
            {
                return _currentSearchResult;
            }

            set
            {
                if (value < 0 || value > TotalSearchResults)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(CurrentSearchResult),
                        $"{nameof(CurrentSearchResult)} must not be negative or greater than the total number of results");
                }
                else
                {
                    _currentSearchResult = value;
                    _counter = string.Concat(_currentSearchResult.ToString(), "/", _totalSearchResults.ToString());
                }
            }
        }
        private int _currentSearchResult = 0;

        public string Counter => _counter;
        private string _counter = "0/0";
    }
}
