namespace TranslatorDesktopApp.Explorer.OpenCL
{
	public partial class DeviceSelection : Form
	{
		public DeviceSelection()
		{
			UseWaitCursor= false;
			InitializeComponent();
		}

		public DeviceSelection(string[] deviceNames) : this()
		{
			deviceList.Items.AddRange(deviceNames);
		}

		public int SelectedDeviceIndex => deviceList.SelectedIndex;
	}
}
