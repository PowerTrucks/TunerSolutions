using System;

namespace Vehicle_Applications
{
	public class CRC32
	{
		private uint[] table;

		public uint ComputeChecksum(byte[] bytes)
		{
			uint num = uint.MaxValue;
			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = (byte)((num & 0xFFu) ^ bytes[i]);
				num = (num >> 8) ^ table[b];
			}
			return ~num;
		}

		public uint ComputeChecksum(byte[] bytes, int From, int To)
		{
			uint num = uint.MaxValue;
			for (int i = From; i < To; i++)
			{
				byte b = (byte)((num & 0xFFu) ^ bytes[i]);
				num = (num >> 8) ^ table[b];
			}
			return ~num;
		}

		public byte[] ComputeChecksumBytes(byte[] bytes)
		{
			return BitConverter.GetBytes(ComputeChecksum(bytes));
		}

		public byte[] ComputeChecksumBytes(byte[] bytes, int From, int To)
		{
			return BitConverter.GetBytes(ComputeChecksum(bytes, From, To));
		}

		public CRC32()
		{
			uint num = 3988292384u;
			table = new uint[256];
			uint num2 = 0u;
			for (uint num3 = 0u; num3 < table.Length; num3++)
			{
				num2 = num3;
				for (int num4 = 8; num4 > 0; num4--)
				{
					num2 = (((num2 & 1) != 1) ? (num2 >> 1) : ((num2 >> 1) ^ num));
				}
				table[num3] = num2;
			}
		}
	}
}
