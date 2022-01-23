using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator.StoryExplorerForm
{
    public partial class StoryExplorer : Form
    {
        private readonly ContextProvider Context;
        public StoryExplorer()
        {
            InitializeComponent();
            Context = new ContextProvider("", ExplorerPanel);
            Context.ParseFile();
        }
    }
}
