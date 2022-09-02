using System.Collections.Generic;

namespace rp1210
{
	public class DeviceInfo
	{
		public BaudRate SelectedRate;

		public short DeviceId { get; set; }

		public string DeviceDescription { get; set; }

		public string DeviceName { get; set; }

		public string DeviceParams { get; set; }

		public List<ProtocolInfo> SupportedProtocols { get; set; }

		public DeviceInfo()
		{
			SupportedProtocols = new List<ProtocolInfo>();
		}

		public override string ToString()
		{
			return DeviceName + " " + DeviceDescription;
		}
	}
}
