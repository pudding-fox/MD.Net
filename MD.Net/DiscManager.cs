using MD.Net.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyJson;

namespace MD.Net
{
    public class DiscManager : IDiscManager
    {
        public DiscManager(IToolManager toolManager, IFormatValidator formatValidator)
        {
            this.ToolManager = toolManager;
            this.FormatValidator = formatValidator;
        }

        public IToolManager ToolManager { get; private set; }

        public IFormatValidator FormatValidator { get; private set; }

        public IDisc GetDisc(IDevice device)
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI);
            var code = this.ToolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                this.ToolManager.Throw(process, error);
            }
            return this.GetDisc(device, output);
        }

        protected IDisc GetDisc(IDevice device, string output)
        {
            if (string.IsNullOrEmpty(output) || output.Contains(Constants.NETMDCLI_NO_DEVICE, true) || output.Contains(Constants.NETMDCLI_POLL_FAILED, true))
            {
                return default(IDisc);
            }
            var disc = output.FromJson<_Disc>();
            if (!string.Equals(device.Name, disc.device, StringComparison.OrdinalIgnoreCase))
            {
                return default(IDisc);
            }
            return new Disc(
                disc.title,
                Utility.GetTimeSpan(disc.recordedTime),
                Utility.GetTimeSpan(disc.totalTime),
                Utility.GetTimeSpan(disc.availableTime),
                new Tracks(this.FormatValidator, this.GetTracks(disc.tracks))
            );
        }

        private IEnumerable<ITrack> GetTracks(IEnumerable<_Track> tracks)
        {
            if (tracks == null)
            {
                return Enumerable.Empty<ITrack>();
            }
            return tracks.Select(track => new Track(
                track.no,
                Utility.GetProtection(track.protect),
                Utility.GetCompression(track.bitrate),
                Utility.GetTimeSpan(track.time),
                track.name
            ));
        }

        public IResult ApplyActions(IDevice device, IActions actions, IStatus status, bool validate)
        {
            var message = default(string);
            if (validate)
            {
                if (!this.ValidateDisc(device, actions.CurrentDisc, out message))
                {
                    return Result.Failure(message);
                }
            }
            Parallel.ForEach(actions, new ParallelOptions() { MaxDegreeOfParallelism = Settings.Threads }, action =>
            {
                action.Prepare(this.ToolManager, status);
            });
            var position = 0;
            var count = actions.Count;
            foreach (var action in actions)
            {
                status.Update(action.Description, position, count, StatusType.Action);
                try
                {
                    action.Apply(this.ToolManager, status);
                    action.Commit();
                }
                catch (Exception e)
                {
                    message = e.Message;
                    break;
                }
                finally
                {
                    position++;
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                return Result.Failure(message);
            }
            return Result.Success;
        }

        protected virtual bool ValidateDisc(IDevice device, IDisc expectedDisc, out string message)
        {
            var actualDisc = this.GetDisc(device);
            if (!actualDisc.Equals(expectedDisc))
            {
                message = Strings.Error_UnexpectedDisc;
                return false;
            }
            message = string.Empty;
            return true;
        }

#pragma warning disable 0169, 0649

        private struct _Disc
        {
            public string device;

            public string title;

            public string recordedTime;

            public string totalTime;

            public string availableTime;

            public List<_Track> tracks;
        }

        private struct _Track
        {
            public int no;

            public string protect;

            public string bitrate;

            public string time;

            public string name;
        }

#pragma warning restore 0169, 0649

    }
}
