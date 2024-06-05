using System;
using System.Linq;

namespace MD.Net
{
    public class Capacity : ICapacity
    {
        public const int SP_RATE = 292;

        public const int LP2_RATE = 132;

        public const int LP4_RATE = 66;

        public Capacity(IDisc disc)
        {
            this.Disc = disc;
        }

        public IDisc Disc { get; private set; }

        public int PercentUsed
        {
            get
            {
                return Convert.ToInt32((this.GetUsage() / this.Disc.TotalTime.TotalMilliseconds) * 100);
            }
        }

        public int PercentFree
        {
            get
            {
                return 100 - this.PercentUsed;
            }
        }

        protected virtual double GetUsage()
        {
            return this.Disc.Tracks.Sum(this.GetUsage);
        }

        protected virtual double GetUsage(ITrack track)
        {
            var time = track.Time.TotalMilliseconds;
            switch (track.Compression)
            {
                case Compression.LP2:
                    time /= ((double)SP_RATE / LP2_RATE);
                    break;
                case Compression.LP4:
                    time /= ((double)SP_RATE / LP4_RATE);
                    break;
            }
            return time;
        }
    }
}
