using System;

namespace MD.Net
{
    public interface IPlaybackManager
    {
        bool GetStatus(IDevice device, out string name, out TimeSpan position);

        void Play(IDevice device);

        void Play(IDevice device, ITrack track);

        void Next(IDevice device);

        void Previous(IDevice device);

        void Pause(IDevice device);

        void Stop(IDevice device);

        void Seek(IDevice device, TimeSpan position);
    }
}
