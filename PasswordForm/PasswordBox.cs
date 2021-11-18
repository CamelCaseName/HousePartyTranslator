using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public partial class Password : Form
    {
        public Password()
        {
            InitializeComponent();
        }

        public string GetPassword()
        {
            return PasswordTextBox.Text;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {

        }
    }
}
