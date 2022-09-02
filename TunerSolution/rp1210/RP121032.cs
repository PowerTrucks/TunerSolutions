using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace rp1210
{
	public class RP121032 : IDisposable
	{
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210ClientConnect(IntPtr hwndClient, short nDeviceId, StringBuilder fpchProtocol, int lSendBuffer, int lReceiveBuffer, short nIsAppPacketizingIncomingMsgs);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210ClientDisconnect(short nClientID);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210SendMessage(short nClientID, byte[] fpchClientMessage, short nMessageSize, short nNotifyStatusOnTx, short nBlockOnSend);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210ReadMessage(short nClientID, byte[] fpchAPIMessage, short nBufferSize, short nBlockOnSend);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210SendCommand(short nCommandNumber, short nClientID, byte[] fpchClientCommand, short nMessageSize);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate void RP1210ReadVersion(StringBuilder fpchDLLMajorVersion, StringBuilder fpchDLLMinorVersion, StringBuilder fpchAPIMajorVersion, StringBuilder fpchAPIMinorVersion);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210ReadDetailedVersion(short nClientID, StringBuilder fpchAPIVersionInfo, StringBuilder fpchDLLVersionInfo, StringBuilder fpchFWVersionInfo);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210GetHardwareStatus(short nClientID, StringBuilder fpchClientInfo, short nInfoSize, short nBlockOnRequest);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate short RP1210GetErrorMsg(short err_code, StringBuilder fpchMessage);

		private class Win32API
		{
			[DllImport("kernel32.dll")]
			public static extern IntPtr LoadLibrary(string dllToLoad);

			[DllImport("kernel32.dll")]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

			[DllImport("kernel32.dll")]
			public static extern bool FreeLibrary(IntPtr hModule);
		}

		public const string RP1210_INI = "rp121032.ini";

		public const byte BLOCKING_IO = 1;

		public const byte NON_BLOCKING_IO = 0;

		public const byte CONVERTED_MODE = 1;

		public const byte RAW_MODE = 0;

		public const uint MAX_J1708_MESSAGE_LENGTH = 508u;

		public const uint MAX_J1939_MESSAGE_LENGTH = 1796u;

		public const uint MAX_J1850_MESSAGE_LENGTH = 508u;

		public const uint CAN_DATA_SIZE = 8u;

		public const byte ECHO_OFF = 0;

		public const byte ECHO_ON = 1;

		public const byte RECEIVE_ON = 1;

		public const byte RECEIVE_OFF = 0;

		public const uint FILTER_PGN = 1u;

		public const uint FILTER_PRIORITY = 2u;

		public const uint FILTER_SOURCE = 4u;

		public const uint FILTER_DESTINATION = 8u;

		public const byte SILENT_J1939_CLAIM = 0;

		public const byte PASS_J1939_CLAIM_MESSAGES = 1;

		public const byte STANDARD_CAN = 0;

		public const byte EXTENDED_CAN = 1;

		private const string RP1210_CLIENT_CONNECT = "RP1210_ClientConnect";

		private const string RP1210_CLIENT_DISCONNECT = "RP1210_ClientDisconnect";

		private const string RP1210_GET_ERROR_MSG = "RP1210_GetErrorMsg";

		private const string RP1210_GET_HARDWARE_STATUS = "RP1210_GetHardwareStatus";

		private const string RP1210_READ_DETAILED_VERSION = "RP1210_ReadDetailedVersion";

		private const string RP1210_READ_MESSAGE = "RP1210_ReadMessage";

		private const string RP1210_READ_VERSION = "RP1210_ReadVersion";

		private const string RP1210_SEND_COMMAND = "RP1210_SendCommand";

		private const string RP1210_SEND_MESSAGE = "RP1210_SendMessage";

		private bool disposed;

		private short _connectedDevice = -1;

		private IntPtr pDLL;

		private IntPtr fpRP1210_ClientConnect;

		private IntPtr fpRP1210_ClientDisconnect;

		private IntPtr fpRP1210_SendMessage;

		private IntPtr fpRP1210_ReadMessage;

		private IntPtr fpRP1210_SendCommand;

		private IntPtr fpRP1210_ReadVersion;

		private IntPtr fpRP1210_ReadDetailedVersion;

		private IntPtr fpRP1210_GetHardwareStatus;

		private IntPtr fpRP1210_GetErrorMsg;

		private RP1210ClientConnect pRP1210_ClientConnect;

		private RP1210ClientDisconnect pRP1210_ClientDisconnect;

		private RP1210SendMessage pRP1210_SendMessage;

		private RP1210ReadMessage pRP1210_ReadMessage;

		private RP1210SendCommand pRP1210_SendCommand;

		private RP1210ReadVersion pRP1210_ReadVersion;

		private RP1210ReadDetailedVersion pRP1210_ReadDetailedVersion;

		private RP1210GetHardwareStatus pRP1210_GetHardwareStatus;

		private RP1210GetErrorMsg pRP1210_GetErrorMsg;

		private RP1210BDriverInfo _DriverInfo;

		public short nClientID => _connectedDevice;

		public RP1210BDriverInfo DriverInfo => _DriverInfo;

		public short MaxBufferSize { get; set; }

		public RP121032(string NameOfRP1210DLL)
		{
			string dllToLoad = Environment.GetEnvironmentVariable("SystemRoot") + "\\System32\\" + NameOfRP1210DLL + ".dll";
			string strDeviceIniPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\" + NameOfRP1210DLL + ".ini";
			pDLL = Win32API.LoadLibrary(dllToLoad);
			if (!(pDLL == IntPtr.Zero))
			{
				fpRP1210_ClientConnect = Win32API.GetProcAddress(pDLL, "RP1210_ClientConnect");
				fpRP1210_ClientDisconnect = Win32API.GetProcAddress(pDLL, "RP1210_ClientDisconnect");
				fpRP1210_SendMessage = Win32API.GetProcAddress(pDLL, "RP1210_SendMessage");
				fpRP1210_ReadMessage = Win32API.GetProcAddress(pDLL, "RP1210_ReadMessage");
				fpRP1210_SendCommand = Win32API.GetProcAddress(pDLL, "RP1210_SendCommand");
				fpRP1210_ReadVersion = Win32API.GetProcAddress(pDLL, "RP1210_ReadVersion");
				fpRP1210_ReadDetailedVersion = Win32API.GetProcAddress(pDLL, "RP1210_ReadDetailedVersion");
				fpRP1210_GetHardwareStatus = Win32API.GetProcAddress(pDLL, "RP1210_GetHardwareStatus");
				fpRP1210_GetErrorMsg = Win32API.GetProcAddress(pDLL, "RP1210_GetErrorMsg");
				pRP1210_ClientConnect = (RP1210ClientConnect)Marshal.GetDelegateForFunctionPointer(fpRP1210_ClientConnect, typeof(RP1210ClientConnect));
				pRP1210_ClientDisconnect = (RP1210ClientDisconnect)Marshal.GetDelegateForFunctionPointer(fpRP1210_ClientDisconnect, typeof(RP1210ClientDisconnect));
				pRP1210_SendMessage = (RP1210SendMessage)Marshal.GetDelegateForFunctionPointer(fpRP1210_SendMessage, typeof(RP1210SendMessage));
				pRP1210_ReadMessage = (RP1210ReadMessage)Marshal.GetDelegateForFunctionPointer(fpRP1210_ReadMessage, typeof(RP1210ReadMessage));
				pRP1210_SendCommand = (RP1210SendCommand)Marshal.GetDelegateForFunctionPointer(fpRP1210_SendCommand, typeof(RP1210SendCommand));
				pRP1210_ReadVersion = (RP1210ReadVersion)Marshal.GetDelegateForFunctionPointer(fpRP1210_ReadVersion, typeof(RP1210ReadVersion));
				pRP1210_ReadDetailedVersion = (RP1210ReadDetailedVersion)Marshal.GetDelegateForFunctionPointer(fpRP1210_ReadDetailedVersion, typeof(RP1210ReadDetailedVersion));
				pRP1210_GetHardwareStatus = (RP1210GetHardwareStatus)Marshal.GetDelegateForFunctionPointer(fpRP1210_GetHardwareStatus, typeof(RP1210GetHardwareStatus));
				pRP1210_GetErrorMsg = (RP1210GetErrorMsg)Marshal.GetDelegateForFunctionPointer(fpRP1210_GetErrorMsg, typeof(RP1210GetErrorMsg));
				_DriverInfo = LoadDeviceParameters(strDeviceIniPath);
			}
			MaxBufferSize = 255;
		}

		public void RP1210_ClientConnect(short nDeviceId, StringBuilder fpchProtocol, int lSendBuffer, int lReceiveBuffer, short nIsAppPacketizingIncomingMsgs)
		{
			short num = pRP1210_ClientConnect(IntPtr.Zero, nDeviceId, fpchProtocol, lSendBuffer, lReceiveBuffer, nIsAppPacketizingIncomingMsgs);
			if (num >= 0 && num <= 127)
			{
				_connectedDevice = num;
			}
			else if (num != 275)
			{
				throw new Exception("ClientConnect Failed: " + Convert.ToString(num) + " " + RP1210_GetErrorMsg((RP1210_Returns)num));
			}
		}

		public RP1210_Returns RP1210_ClientDisconnect()
		{
			return (RP1210_Returns)pRP1210_ClientDisconnect(_connectedDevice);
		}

		public RP1210_Returns RP1210_SendMessage(byte[] fpchClientMessage, short nMessageSize, short nNotifyStatusOnTx, short nBlockOnSend)
		{
			RP1210_Returns rP1210_Returns;
			if (_connectedDevice >= 0)
			{
				rP1210_Returns = (RP1210_Returns)pRP1210_SendMessage(_connectedDevice, fpchClientMessage, nMessageSize, nNotifyStatusOnTx, nBlockOnSend);
				if ((short)rP1210_Returns > 127)
				{
					throw new Exception("SendMessage Failed: " + RP1210_GetErrorMsg(rP1210_Returns));
				}
				return rP1210_Returns;
			}
			rP1210_Returns = RP1210_Returns.ERR_CLIENT_DISCONNECTED;
			throw new Exception("Device Not Connected");
		}

		public byte[] RP1210_ReadMessage(short nBlockOnSend)
		{
			byte[] array = new byte[MaxBufferSize];
			RP1210_Returns rP1210_Returns = (RP1210_Returns)pRP1210_ReadMessage(_connectedDevice, array, MaxBufferSize, nBlockOnSend);
			if ((ushort)rP1210_Returns > MaxBufferSize)
			{
				throw new Exception("ReadMessage Failed: " + RP1210_GetErrorMsg(rP1210_Returns));
			}
			byte[] array2 = new byte[(short)rP1210_Returns];
			Array.Copy(array, array2, (short)rP1210_Returns);
			return array2;
		}

		public void RP1210_SendCommand(RP1210_Commands nCommandNumber, byte[] fpchClientCommand, short nMessageSize)
		{
			short num = pRP1210_SendCommand((short)nCommandNumber, _connectedDevice, fpchClientCommand, nMessageSize);
			if (num != 0)
			{
				throw new Exception("SendCommand Failed: " + RP1210_GetErrorMsg((RP1210_Returns)num));
			}
		}

		public string[] RP1210_ReadVersion()
		{
			string[] array = new string[2];
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			pRP1210_ReadVersion(stringBuilder, stringBuilder2, stringBuilder3, stringBuilder4);
			array[0] = stringBuilder.ToString() + "." + stringBuilder2.ToString();
			array[1] = stringBuilder3.ToString() + "." + stringBuilder4.ToString();
			return array;
		}

		public string[] RP1210_ReadDetailedVersion()
		{
			string[] array = new string[3];
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			RP1210_Returns rP1210_Returns = (RP1210_Returns)pRP1210_ReadDetailedVersion(_connectedDevice, stringBuilder, stringBuilder2, stringBuilder3);
			if ((short)rP1210_Returns > 127)
			{
				throw new Exception("ReadDetailedVersion Failed: " + RP1210_GetErrorMsg(rP1210_Returns));
			}
			array[0] = stringBuilder2.ToString();
			array[1] = stringBuilder.ToString();
			array[2] = stringBuilder3.ToString();
			return array;
		}

		public RP1210_Returns RP1210_GetHardwareStatus(StringBuilder fpchClientInfo, short nInfoSize, short nBlockOnRequest)
		{
			return (RP1210_Returns)pRP1210_GetHardwareStatus(_connectedDevice, fpchClientInfo, nInfoSize, nBlockOnRequest);
		}

		public string RP1210_GetErrorMsg(RP1210_Returns err_code)
		{
			StringBuilder stringBuilder = new StringBuilder();
			pRP1210_GetErrorMsg((short)err_code, stringBuilder);
			return stringBuilder.ToString();
		}

		public static List<string> ScanForDrivers()
		{
			string iniPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\rp121032.ini";
			try
			{
				return new List<string>(new IniParser(iniPath).GetSetting("RP1210Support", "APIImplementations").Split(new char[1] { ',' }));
			}
			catch
			{
				return null;
			}
		}

		public static RP1210BDriverInfo LoadDeviceParameters(string strDeviceIniPath)
		{
			RP1210BDriverInfo rP1210BDriverInfo = new RP1210BDriverInfo();
			IniParser iniParser = new IniParser(strDeviceIniPath);
			try
			{
				rP1210BDriverInfo.DriverVersion = iniParser.GetSetting("VendorInformation", "Version");
			}
			catch
			{
				rP1210BDriverInfo.DriverVersion = "Unknown";
			}
			try
			{
				rP1210BDriverInfo.VendorName = iniParser.GetSetting("VendorInformation", "Name");
			}
			catch
			{
				rP1210BDriverInfo.VendorName = "Unknown";
			}
			try
			{
				rP1210BDriverInfo.RP1210Version = iniParser.GetSetting("VendorInformation", "RP1210");
			}
			catch
			{
				rP1210BDriverInfo.RP1210Version = "B";
			}
			try
			{
				rP1210BDriverInfo.TimestampWeight = Convert.ToInt16(iniParser.GetSetting("VendorInformation", "TimestampWeight"));
			}
			catch
			{
				rP1210BDriverInfo.TimestampWeight = 1000;
			}
			try
			{
				string setting = iniParser.GetSetting("VendorInformation", "CANFormatsSupported");
				rP1210BDriverInfo.CANFormatsSupported = new List<short>(Array.ConvertAll(setting.Split(new char[1] { ',' }), (string x) => Convert.ToInt16(x)));
			}
			catch
			{
				rP1210BDriverInfo.CANFormatsSupported = new List<short>(new short[1] { 4 });
			}
			try
			{
				string setting2 = iniParser.GetSetting("VendorInformation", "J1939FormatsSupported");
				rP1210BDriverInfo.J1939FormatsSupported = new List<short>(Array.ConvertAll(setting2.Split(new char[1] { ',' }), (string x) => Convert.ToInt16(x)));
			}
			catch
			{
				rP1210BDriverInfo.J1939FormatsSupported = new List<short>(new short[1] { 2 });
			}
			try
			{
				string setting3 = iniParser.GetSetting("VendorInformation", "J1708FormatsSupported");
				rP1210BDriverInfo.J1708FormatsSupported = new List<short>(Array.ConvertAll(setting3.Split(new char[1] { ',' }), (string x) => Convert.ToInt16(x)));
			}
			catch
			{
				rP1210BDriverInfo.J1708FormatsSupported = new List<short>(new short[1] { 2 });
			}
			try
			{
				string setting4 = iniParser.GetSetting("VendorInformation", "ISO15765FormatsSupported");
				rP1210BDriverInfo.CANFormatsSupported = new List<short>(Array.ConvertAll(setting4.Split(new char[1] { ',' }), (string x) => Convert.ToInt16(x)));
			}
			catch
			{
				rP1210BDriverInfo.ISO15765FormatsSupported = new List<short>(new short[1] { 2 });
			}
			foreach (string item in new List<string>(iniParser.GetSetting("VendorInformation", "Devices").Split(new char[1] { ',' })))
			{
				DeviceInfo deviceInfo = new DeviceInfo();
				deviceInfo.DeviceId = Convert.ToInt16(iniParser.GetSetting("DeviceInformation" + item, "DeviceId"));
				deviceInfo.DeviceDescription = iniParser.GetSetting("DeviceInformation" + item, "DeviceDescription");
				deviceInfo.DeviceName = iniParser.GetSetting("DeviceInformation" + item, "DeviceName");
				deviceInfo.DeviceParams = iniParser.GetSetting("DeviceInformation" + item, "DeviceParams");
				rP1210BDriverInfo.RP1210Devices.Add(deviceInfo);
			}
			foreach (string item2 in new List<string>(iniParser.GetSetting("VendorInformation", "Protocols").Split(new char[1] { ',' })))
			{
				ProtocolInfo protocolInfo = new ProtocolInfo();
				protocolInfo.ProtocolString = iniParser.GetSetting("ProtocolInformation" + item2, "ProtocolString");
				protocolInfo.ProtocolDescription = iniParser.GetSetting("ProtocolInformation" + item2, "ProtocolDescription");
				protocolInfo.ProtocolParams = iniParser.GetSetting("ProtocolInformation" + item2, "ProtocolParams");
				try
				{
					string setting5 = iniParser.GetSetting("ProtocolInformation" + item2, "ProtocolSpeed");
					protocolInfo.ProtocolSpeed = new List<string>(setting5.Split(new char[1] { ',' }));
				}
				catch
				{
				}
				foreach (string devTemp in new List<string>(iniParser.GetSetting("ProtocolInformation" + item2, "Devices").Split(new char[1] { ',' })))
				{
					rP1210BDriverInfo.RP1210Devices.Find((DeviceInfo x) => x.DeviceId == Convert.ToInt16(devTemp))!.SupportedProtocols.Add(protocolInfo);
				}
			}
			return rP1210BDriverInfo;
		}

		public static J1939Message DecodeJ1939Message(byte[] message)
		{
			J1939Message j1939Message = new J1939Message();
			j1939Message.TimeStamp = (uint)((message[0] << 24) + (message[1] << 16) + (message[2] << 8) + message[3]);
			j1939Message.PGN = (ushort)((message[6] << 16) + (message[5] << 8) + message[4]);
			j1939Message.Priority = message[7];
			j1939Message.SourceAddress = message[8];
			j1939Message.DestinationAddress = message[9];
			j1939Message.dataLength = (ushort)(message.Length - 10);
			j1939Message.Data = new byte[j1939Message.dataLength];
			Array.Copy(message, 10, j1939Message.Data, 0, j1939Message.dataLength);
			return j1939Message;
		}

		public static byte[] EncodeJ1939Message(J1939Message MessageToEncode)
		{
			byte b = 0;
			byte[] array = new byte[MessageToEncode.dataLength + 6];
			array[b++] = (byte)(MessageToEncode.PGN & 0xFFu);
			array[b++] = (byte)((uint)(MessageToEncode.PGN >> 8) & 0xFFu);
			array[b++] = (byte)((uint)(MessageToEncode.PGN >> 16) & 0xFFu);
			array[b++] |= MessageToEncode.Priority;
			array[b++] = (byte)MessageToEncode.SourceAddress;
			array[b++] = (byte)MessageToEncode.DestinationAddress;
			byte[] data = MessageToEncode.Data;
			foreach (byte b2 in data)
			{
				array[b++] = b2;
			}
			return array;
		}

		public static J1587Message DecodeJ1587Message(byte[] message)
		{
			J1587Message j1587Message = new J1587Message();
			j1587Message.TimeStamp = (uint)((message[0] << 24) + (message[1] << 16) + (message[2] << 8) + message[3]);
			j1587Message.MID = message[4];
			j1587Message.PID = message[5];
			j1587Message.DataLength = (ushort)(message.Length - 6);
			j1587Message.Data = new byte[j1587Message.DataLength];
			Array.Copy(message, 6, j1587Message.Data, 0, j1587Message.DataLength);
			return j1587Message;
		}

		public static byte[] EncodeJ1587Message(J1587Message MessageToEncode)
		{
			byte b = 0;
			byte[] array = new byte[MessageToEncode.DataLength + 3];
			array[b++] = MessageToEncode.Priority;
			array[b++] = MessageToEncode.MID;
			array[b++] = MessageToEncode.PID;
			byte[] data = MessageToEncode.Data;
			foreach (byte b2 in data)
			{
				array[b++] = b2;
			}
			return array;
		}

		public void Dispose()
		{
			if (!disposed)
			{
				try
				{
					RP1210_SendCommand(RP1210_Commands.RP1210_Reset_Device, null, 0);
					RP1210_ClientDisconnect();
				}
				catch
				{
				}
				Win32API.FreeLibrary(pDLL);
			}
			disposed = true;
		}

		~RP121032()
		{
			Dispose();
		}
	}
}
