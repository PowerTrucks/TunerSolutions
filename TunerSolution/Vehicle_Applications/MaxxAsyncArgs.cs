using rp1210;

namespace Vehicle_Applications
{
	public class MaxxAsyncArgs
	{
		public string FlashFile;

		public MaxxModuleType Module;

		public DeviceInfo SelectedDevice;

		public bool FullFlash;

		public MaxxAsyncArgs(MaxxModuleType module, string flashFile, DeviceInfo selectedDevice, bool fullFlash)
		{
			FlashFile = flashFile;
			Module = module;
			SelectedDevice = selectedDevice;
			FullFlash = fullFlash;
		}
	}
}
