using MD.Net.Resources;

namespace MD.Net
{
    public class RemoveTrackAction : TrackAction
    {
        public RemoveTrackAction(IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack track) : base(device, currentDisc, updatedDisc, track, Track.None)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format(Strings.RemoveTrackAction_Description, this.CurrentTrack.Name);
            }
        }

        public override void Apply(IToolManager toolManager, IStatus status)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} {1}", Constants.NETMDCLI_DELETE, this.CurrentTrack.Position));
            var code = toolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                toolManager.Throw(process, error);
            }
        }

        public override void Commit()
        {
            this.CurrentDisc.Tracks.Remove(this.CurrentTrack);
            base.Commit();
        }
    }
}
