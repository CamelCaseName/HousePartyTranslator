namespace Translator.Explorer.OpenCL
{
	public partial class DeviceSelection : Form
	{
		public DeviceSelection()
		{
			InitializeComponent();
		}

		public DeviceSelection(string[] deviceNames, string preselectedDevice) : this()
		{
			deviceList.Items.AddRange(deviceNames);
			deviceList.SelectedItem = preselectedDevice;
		}

		public int SelectedDeviceIndex => deviceList.SelectedIndex;

		public bool GotSelection { get; private set; }

		private void SubmitButton_Click(object sender, EventArgs e)
		{
			//disable interaction
			deviceList.Enabled = false;
			DialogResult = DialogResult.OK;
		}

		private void OurCancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
