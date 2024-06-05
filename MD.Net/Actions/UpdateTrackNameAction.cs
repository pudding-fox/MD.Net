using MD.Net.Resources;

namespace MD.Net
{
    public class UpdateTrackNameAction : TrackAction
    {
        public UpdateTrackNameAction(IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack currentTrack, ITrack updatedTrack) : base(device, currentDisc, updatedDisc, currentTrack, updatedTrack)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format(Strings.UpdateTrackNameAction_Description, this.UpdatedTrack.Name);
            }
        }

        public override void Apply(IToolManager toolManager, IStatus status)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} {1} {2}", Constants.NETMDCLI_RENAME, this.CurrentTrack.Position, this.UpdatedTrack.Name));
            var code = toolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                toolManager.Throw(process, error);
            }
        }

        public override void Commit()
        {
            this.CurrentTrack.Name = this.UpdatedTrack.Name;
            base.Commit();
        }
    }
}
