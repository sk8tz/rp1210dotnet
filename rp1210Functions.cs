using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace rp1210
{
    class dgdFileReplay
    {
        public RP121032 J1939Instance { get; set; }
        public RP121032 J1587Instance { get; set; }
        public long TimeOffsetMs { get; set; }

        public ConcurrentQueue<J1939Message> TXQueue { get; set; }

        public bool Running { get; set; }

        public void dgdReplay()
        {
            Stopwatch timeKeeper = Stopwatch.StartNew();

            while (Running)
            {
                J1939Message TXData;
                if (TXQueue.TryPeek(out TXData))
                {
                    if (timeKeeper.ElapsedMilliseconds > (TXData.TimeStamp - TimeOffsetMs))
                    {
                        if (TXQueue.TryDequeue(out TXData))
                        {
                            byte[] data2send = RP121032.EncodeJ1939Message(TXData);
                            J1939Instance.RP1210_SendMessage(data2send, (short)data2send.Length, 0, RP121032.BLOCKING_IO);
                        }
                    }
                }
            }
        }

        public dgdFileReplay()
        {
        }
    }
}
