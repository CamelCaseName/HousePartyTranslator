using System;
using System.Windows.Forms;

namespace Translator.Desktop.UI
{
    /// <summary>
    /// A Class providing a Popup Form with a password field and a Submit button whcih returns an Button.OK state when clicked.
    /// </summary>
    public partial class Password : Form
    {
        public bool PasswordSubmitted { get; private set; } = false;
        /// <summary>
        /// The default constructor. Takes no arguments.
        /// </summary>
        public Password()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Returns the password entered into the field.
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            return PasswordTextBox.Text;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            PasswordSubmitted = true;
        }
    }
}
