using System;
using System.Runtime.InteropServices;

namespace rp1210
{
	public class J1939Message
	{
		public uint TimeStamp { get; set; }

		public short SourceAddress { get; set; }

		public short DestinationAddress { get; set; }

		public byte Priority { get; set; }

		public ushort PGN { get; set; }

		public ushort dataLength { get; set; }

		public byte[] Data { get; set; }

		public J1939Message()
		{
		}

		public J1939Message(uint pTimeStamp, short pSource, short pDestination, byte pPriority, ushort pPGN, ushort pDateLength, byte[] pData)
		{
			TimeStamp = pTimeStamp;
			SourceAddress = pSource;
			DestinationAddress = pDestination;
			Priority = pPriority;
			PGN = pPGN;
			dataLength = pDateLength;
			Data = pData;
		}

		public static byte[] SerializeMessage<T>(T msg) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(msg, intPtr, fDeleteOld: true);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return array;
		}

		public static T DeserializeMsg<T>(byte[] data) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(data, 0, intPtr, num);
			T result = (T)Marshal.PtrToStructure(intPtr, typeof(T));
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public byte[] ToArray()
		{
			byte b = 0;
			byte[] array = new byte[dataLength + 6];
			array[b++] = (byte)(PGN & 0xFFu);
			array[b++] = (byte)((uint)(PGN >> 8) & 0xFFu);
			array[b++] = (byte)((uint)(PGN >> 16) & 0xFFu);
			array[b++] |= Priority;
			array[b++] = (byte)SourceAddress;
			array[b++] = (byte)DestinationAddress;
			byte[] data = Data;
			foreach (byte b2 in data)
			{
				array[b++] = b2;
			}
			return array;
		}
	}
}
