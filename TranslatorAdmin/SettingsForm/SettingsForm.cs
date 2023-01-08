using TranslatorAdmin.Properties;

namespace Translator.SettingsForm
{
	/// <summary>
	/// Form with a propertygrid to edit application settings
	/// </summary>
	public partial class SettingsForm : Form
	{
		/// <summary>
		/// The constructor for the settongs form
		/// </summary>
		public SettingsForm()
		{
			InitializeComponent();
			DisplaySettings();
		}

		/// <summary>
		/// Displays the settings in the window
		/// </summary>
		public void DisplaySettings()
		{
			propertyGrid.PropertySort = PropertySort.CategorizedAlphabetical;
			propertyGrid.SelectedObject = Settings.Default;
		}

		/// <summary>
		/// Delegate to save settings before the form is closed
		/// </summary>
		/// <param name="sender">Object who initiated the event</param>
		/// <param name="e">Arguments sent with the event</param>
		public void SettingsForm_closing(object sender, EventArgs e)
		{
			((Settings)propertyGrid.SelectedObject).Save();

		}
	}
}
