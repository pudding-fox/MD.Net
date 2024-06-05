using System;
using System.IO;

namespace MD.Net
{
    public class PlaybackManager : IPlaybackManager
    {
        public PlaybackManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public bool GetStatus(IDevice device, out string name, out TimeSpan position)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_STATUS);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
            name = default(string);
            position = default(TimeSpan);
            if (string.IsNullOrEmpty(output))
            {
                return false;
            }
            using (var reader = new StringReader(output))
            {
                var line = default(string);
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(Constants.NETMDCLI_CURRENT_TRACK, StringComparison.OrdinalIgnoreCase))
                    {
                        name = line.Substring(Constants.NETMDCLI_CURRENT_TRACK.Length);
                    }
                    else if (line.StartsWith(Constants.NETMDCLI_CURRENT_POSITION, StringComparison.OrdinalIgnoreCase))
                    {
                        position = Utility.GetTimeSpan(line.Substring(Constants.NETMDCLI_CURRENT_POSITION.Length));
                    }
                }
            }
            return !string.IsNullOrEmpty(name) && position != TimeSpan.Zero;
        }

        public void Play(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_PLAY);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }

        public void Play(IDevice device, ITrack track)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_PLAY, track.Position));
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }

        public void Next(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_NEXT);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }

        public void Previous(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_PREVIOUS);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }

        public void Pause(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_PAUSE);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }

        public void Stop(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, Constants.NETMDCLI_STOP);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }

        public void Seek(IDevice device, TimeSpan position)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI, string.Format("{0} {1} {2} {3} {4}", Constants.NETMDCLI_SETTIME, position.Hours, position.Minutes, position.Seconds, Utility.MSToFrames(position.Milliseconds)));
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
        }
    }
}
