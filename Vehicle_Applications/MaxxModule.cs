using System;

namespace Vehicle_Applications
{
	public class MaxxModule
	{
		private MaxxModuleType ECMModule;

		public ushort DiagPGN = 65501;

		public ushort FlashPGN = 65502;

		public ushort Seed = 48869;

		public byte DestAddr;

		public byte[] MaxxBasePacket = new byte[16];

		public ProtocolType Protocol = ProtocolType.J1939;

		public bool HasParameters = true;

		public byte SoftwareIDOffset;

		public MaxxFlashSpace Protective;

		public MaxxFlashSpace ProtectiveContiguous;

		public MaxxFlashSpace Strategy;

		public MaxxFlashSpace StrategyContiguous;

		public MaxxFlashSpace Calibration;

		public MaxxFlashSpace CalibrationContiguous;

		public byte FillValue = byte.MaxValue;

		public static string ErrorString(byte Code)
		{
			return Code switch
			{
				byte.MaxValue => "Error -1: No Response from ECM", 
				0 => "", 
				1 => "Error 1: Undefined Request Command", 
				2 => "Error 2: Password Didn't Match", 
				3 => "Error 3: Engine Running", 
				4 => "Error 4: Vehicle Moving", 
				5 => "Error 5: Programming Error / Memory Failure", 
				6 => "Error 6: Invalid Strategy/Calibration ROM Checksum", 
				7 => "Error 7: Cannot Write A 'Read Only' Field", 
				8 => "Error 8: Programming Mode Timeout", 
				9 => "Error 9: Multiple Faults During Session / Data Block Too Long To Transmit While Not In Programming Mode", 
				10 => "Error 10: Incorrect PP Command / Length of Data Provided is Too Long/Short", 
				11 => "Error 11: Invalid Parameters / Incorrect PP Programming Command", 
				13 => "Error 13: Improper Command Sequence / PP Programming Command Received Outside of PP Programming session", 
				14 => "Error 14: Invalid Password Level Provided", 
				15 => "Error 15: Data Block Too Long To Transmit", 
				16 => "Error 16: Engine Operating Mode Not in 'No-Start mode'", 
				17 => "Error 17: Transparent Parameters Data CRC Mismatch", 
				18 => "Error 18: Flash ROM Segment Not Erased Prior to Programming", 
				20 => "Error 20: Programming Message Packet Received Out of Sequence", 
				_ => "Error " + Code + ": Unknown Return Code", 
			};
		}

		public static ProtocolType MaxxModuleProtocol(MaxxModuleType Input)
		{
			if ((uint)(Input - 6) <= 1u)
			{
				return ProtocolType.J1708;
			}
			return ProtocolType.J1939;
		}

		public MaxxModule(MaxxModuleType InputType)
		{
			ECMModule = InputType;
			Protective = new MaxxFlashSpace(45);
			Strategy = new MaxxFlashSpace(43);
			Calibration = new MaxxFlashSpace(44);
			ProtectiveContiguous = new MaxxFlashSpace(45);
			StrategyContiguous = new MaxxFlashSpace(43);
			CalibrationContiguous = new MaxxFlashSpace(44);
			Protocol = MaxxModuleProtocol(InputType);
			switch (ECMModule)
			{
			case MaxxModuleType.EDC17:
				Protective.Add(16384, 81920);
				Strategy.Add(131072, 2097152);
				Calibration.Add(2162688, 3145728);
				FillValue = 0;
				Seed = 53159;
				break;
			case MaxxModuleType.S3V8:
			case MaxxModuleType.S4I6:
				Strategy.Add(131072, 262080);
				Strategy.Add(786432, 1769408);
				Strategy.Add(4194304, 4718592);
				Calibration.Add(8650816, 9174848);
				Seed = 53159;
				break;
			case MaxxModuleType.S4I6Special:
				Strategy.Add(131072, 262080);
				Strategy.Add(1310720, 4063168);
				Strategy.Add(4194304, 4718592);
				Calibration.Add(8650816, 9699136);
				Seed = 53159;
				break;
			case MaxxModuleType.NECM2:
				Strategy.Add(65600, 393200);
				Calibration.Add(393280, 524272);
				Seed = 53159;
				DestAddr = 128;
				break;
			case MaxxModuleType.IDM2:
				Strategy.Add(32832, 57328);
				Strategy.Add(131072, 196592);
				Calibration.Add(65600, 131056);
				Seed = 53159;
				DestAddr = 143;
				HasParameters = false;
				break;
			case MaxxModuleType.EIM7:
				Strategy.Add(65600, 327664);
				Calibration.Add(393280, 524272);
				Seed = 53159;
				break;
			case MaxxModuleType.S8V8:
				Protective.Add(131072, 262144);
				Strategy.Add(1048576, 3014656);
				Calibration.Add(262400, 1032192);
				Seed = 53159;
				break;
			case MaxxModuleType.EDC7:
				Protective.Add(65536, 261888);
				Strategy.Add(262144, 1703936);
				Calibration.Add(1835008, 2088960);
				DestAddr = 18;
				FlashPGN = 65511;
				DiagPGN = 65510;
				SoftwareIDOffset = 18;
				Seed = 53159;
				break;
			default:
				throw new Exception("Module Type Not Implemented");
			}
		}
	}
}
