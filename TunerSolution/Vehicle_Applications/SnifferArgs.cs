using rp1210;

namespace Vehicle_Applications
{
	public class SnifferArgs
	{
		public DeviceInfo SelectedDevice;

		public bool J1708Sniff;

		public bool J1939Sniff;

		public SnifferArgs(DeviceInfo selectedDevice, bool j1708Sniff, bool j1939Sniff)
		{
			SelectedDevice = selectedDevice;
			J1708Sniff = j1708Sniff;
			J1939Sniff = j1939Sniff;
		}
	}
}
