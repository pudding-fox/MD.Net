using System;
using System.Linq;

namespace MD.Net
{
    public class Disc : IDisc
    {
        public Disc(IDisc disc)
        {
            this.Title = disc.Title;
            this.RecordedTime = disc.RecordedTime;
            this.TotalTime = disc.TotalTime;
            this.AvailableTime = disc.AvailableTime;
            this.Tracks = disc.Tracks.Clone();
            this.Id = disc.Id;
        }

        public Disc(string title, TimeSpan recordedTime, TimeSpan totalTime, TimeSpan availableTime, ITracks tracks)
        {
            this.Title = title;
            this.RecordedTime = recordedTime;
            this.TotalTime = totalTime;
            this.AvailableTime = availableTime;
            this.Tracks = tracks;
            this.Id = GetId(this);
        }

        public string Id { get; private set; }

        public string Title { get; set; }

        public TimeSpan RecordedTime { get; private set; }

        public TimeSpan TotalTime { get; private set; }

        public TimeSpan AvailableTime { get; private set; }

        public ITracks Tracks { get; private set; }

        public IDisc Clone()
        {
            var disc = new Disc(this);
            if (disc.Tracks is Tracks tracks)
            {
                tracks.IsUpdatable = true;
            }
            return disc;
        }

        public ICapacity GetCapacity()
        {
            return new Capacity(this);
        }

        public override int GetHashCode()
        {
            var hashCode = default(int);
            unchecked
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    hashCode += this.Id.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IDisc);
        }

        public virtual bool Equals(IDisc other)
        {
            if (other == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (!string.Equals(this.Id, other.Id, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(Disc a, Disc b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            if (object.ReferenceEquals((object)a, (object)b))
            {
                return true;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Disc a, Disc b)
        {
            return !(a == b);
        }

        public static IDisc None
        {
            get
            {
                return new Disc(string.Empty, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, global::MD.Net.Tracks.None);
            }
        }

        public static string GetId(IDisc disc)
        {
            var id = default(int);
            unchecked
            {
                id += disc.Title.GetHashCode();
                foreach (var track in disc.Tracks)
                {
                    id += track.Id.GetHashCode();
                }
            }
            return Math.Abs(id).ToString();
        }
    }
}
