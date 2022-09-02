using System.Collections.Generic;

namespace Vehicle_Applications
{
	public class MaxxFlashSpace
	{
		public List<FlashAddress> FlashSect;

		public ushort BaseCount;

		public byte SectionID;

		public MaxxFlashSpace(byte InputSection)
		{
			FlashSect = new List<FlashAddress>();
			SectionID = InputSection;
		}

		public void Add(int StartAddr, int EndAddr)
		{
			FlashSect.Add(new FlashAddress(StartAddr, EndAddr));
		}

		public void Clear()
		{
			FlashSect.Clear();
		}
	}
}
