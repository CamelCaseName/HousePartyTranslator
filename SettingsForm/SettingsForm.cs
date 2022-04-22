using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator.SettingsForm
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            DisplaySettings();
        }

        public void DisplaySettings()
        {
            propertyGrid.PropertySort = PropertySort.CategorizedAlphabetical;
            propertyGrid.SelectedObject = Properties.Settings.Default;
        }

        public void SettingsForm_closing(object sender, EventArgs e)
        {
            ((Properties.Settings)propertyGrid.SelectedObject).Save();

        }
    }
}
