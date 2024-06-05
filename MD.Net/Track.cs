using System;

namespace MD.Net
{
    public class Track : ITrack
    {
        public Track(ITrack track)
        {
            this.Position = track.Position;
            this.Protection = track.Protection;
            this.Compression = track.Compression;
            this.Time = track.Time;
            this.Name = track.Name;
            this.Id = track.Id;
        }

        public Track(int position, Protection protection, Compression compression, TimeSpan time, string name)
        {
            this.Position = position;
            this.Protection = protection;
            this.Compression = compression;
            this.Time = time;
            this.Name = name;
            this.Id = GetId(this);
        }

        public string Id { get; private set; }

        public int Position { get; set; }

        public Protection Protection { get; private set; }

        public Compression Compression { get; set; }

        public TimeSpan Time { get; private set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ITrack Clone()
        {
            return new Track(this);
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
            return this.Equals(obj as ITrack);
        }

        public virtual bool Equals(ITrack other)
        {
            if (other == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (!string.Equals(this.Id, other.Id))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(Track a, Track b)
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

        public static bool operator !=(Track a, Track b)
        {
            return !(a == b);
        }

        public static ITrack None
        {
            get
            {
                return new Track(-1, Protection.None, Compression.None, TimeSpan.Zero, string.Empty);
            }
        }

        public static string GetId(ITrack track)
        {
            var id = default(int);
            unchecked
            {
                id += track.Position.GetHashCode();
                if (!string.IsNullOrEmpty(track.Name))
                {
                    id += track.Name.GetHashCode();
                }
                if (track.Time != TimeSpan.Zero)
                {
                    id += track.Time.GetHashCode();
                }
                if (!string.IsNullOrEmpty(track.Location))
                {
                    id += track.Location.GetHashCode();
                }
            }
            return Math.Abs(id).ToString();
        }
    }
}
