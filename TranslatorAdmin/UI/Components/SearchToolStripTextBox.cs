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
        public int TotalSearchResults { get; set; }
        public int CurrentSearchResult { get; set; }
    }
}
