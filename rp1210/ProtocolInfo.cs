using System.Collections.Generic;

namespace rp1210
{
	public class ProtocolInfo
	{
		public string ProtocolString { get; set; }

		public string ProtocolDescription { get; set; }

		public List<string> ProtocolSpeed { get; set; }

		public string ProtocolParams { get; set; }

		public ProtocolInfo()
		{
			ProtocolSpeed = new List<string>();
		}
	}
}
