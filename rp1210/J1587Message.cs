using System;

namespace rp1210
{
	public class J1587Message
	{
		public uint TimeStamp { get; set; }

		public byte Priority { get; set; }

		public byte MID { get; set; }

		public byte PID { get; set; }

		public ushort DataLength { get; set; }

		public byte[] Data { get; set; }

		public byte[] ToArray()
		{
			byte destinationIndex = 0;
			byte[] array = new byte[DataLength + 2];
			array[destinationIndex++] = Priority;
			array[destinationIndex++] = MID;
			array[destinationIndex++] = PID;
			Array.Copy(Data, 0, array, destinationIndex, DataLength);
			return array;
		}
	}
}
