using System;

namespace MD.Net
{
    public interface IDisc : IEquatable<IDisc>
    {
        string Id { get; }

        string Title { get; set; }

        TimeSpan RecordedTime { get; }

        TimeSpan AvailableTime { get; }

        TimeSpan TotalTime { get; }

        ITracks Tracks { get; }

        IDisc Clone();

        ICapacity GetCapacity();
    }
}
