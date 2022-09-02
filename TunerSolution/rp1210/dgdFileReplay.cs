using System.Collections.Concurrent;
using System.Diagnostics;

namespace rp1210
{
	internal class dgdFileReplay
	{
		public RP121032 J1939Instance { get; set; }

		public RP121032 J1587Instance { get; set; }

		public long TimeOffsetMs { get; set; }

		public ConcurrentQueue<J1939Message> TXQueue { get; set; }

		public bool Running { get; set; }

		public void dgdReplay()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (Running)
			{
				if (TXQueue.TryPeek(out var result) && stopwatch.ElapsedMilliseconds > result.TimeStamp - TimeOffsetMs && TXQueue.TryDequeue(out result))
				{
					byte[] array = RP121032.EncodeJ1939Message(result);
					J1939Instance.RP1210_SendMessage(array, (short)array.Length, 0, 1);
				}
			}
		}
	}
}
