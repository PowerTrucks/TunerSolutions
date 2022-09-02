using rp1210;

namespace Vehicle_Applications
{
	public class DetroitMCMAsyncArgs
	{
		public string FlashFile;

		public DetroitMCMModuleType Module;

		public DeviceInfo SelectedDevice;

		public bool FullFlash;

		public DetroitMCMAsyncArgs(DetroitMCMModuleType module, string flashFile, DeviceInfo selectedDevice, bool fullFlash)
		{
			FlashFile = flashFile;
			Module = module;
			SelectedDevice = selectedDevice;
			FullFlash = fullFlash;
		}
	}
}
