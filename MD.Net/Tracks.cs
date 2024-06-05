using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MD.Net
{
    public class Tracks : ITracks
    {
        private Tracks()
        {
            this.Store = new List<ITrack>();
        }

        public Tracks(IFormatValidator formatValidator, IEnumerable<ITrack> tracks)
        {
            this.FormatValidator = formatValidator;
            this.Store = tracks.ToList();
        }

        public IFormatValidator FormatValidator { get; private set; }

        public IList<ITrack> Store { get; private set; }

        internal bool IsUpdatable { get; set; }

        public ITrack this[int position]
        {
            get
            {
                return this.Store.FirstOrDefault(track => track.Position == position);
            }
        }

        public int Count
        {
            get
            {
                return this.Store.Count;
            }
        }

        public ITrack Add(string location, Compression compression)
        {
            var position = 0;
            var time = default(TimeSpan);
            if (this.FormatValidator != null)
            {
                this.FormatValidator.Validate(location, out time);
            }
            if (this.Store.Any())
            {
                position = this.Store.Max(track => track.Position) + 1;
            }
            {
                var track = new Track(position, Protection.None, compression, time, string.Empty)
                {
                    Location = location
                };
                return this.Add(track);
            }
        }

        public ITrack Add(ITrack track)
        {
            this.Store.Add(track);
            return track;
        }

        public void Remove(ITrack track)
        {
            this.Store.Remove(track);
        }

        public void Clear()
        {
            this.Store.Clear();
        }

        public ITracks Clone()
        {
            return new Tracks(this.FormatValidator, this.Store.Select(track => track.Clone()));
        }

        public IEnumerator<ITrack> GetEnumerator()
        {
            return this.Store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Store.GetEnumerator();
        }

        public override int GetHashCode()
        {
            var hashCode = default(int);
            unchecked
            {
                foreach (var track in this)
                {
                    hashCode += track.GetHashCode();
                }
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ITracks);
        }

        public virtual bool Equals(ITracks other)
        {
            if (other == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (!Enumerable.SequenceEqual(this, other))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(Tracks a, Tracks b)
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

        public static bool operator !=(Tracks a, Tracks b)
        {
            return !(a == b);
        }

        public static ITracks None
        {
            get
            {
                return new Tracks();
            }
        }
    }
}
