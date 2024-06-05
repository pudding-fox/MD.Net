namespace MD.Net
{
    public abstract class TrackAction : DiscAction, ITrackAction
    {
        protected TrackAction(IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack currentTrack, ITrack updatedTrack) : base(device, currentDisc, updatedDisc)
        {
            this.CurrentTrack = currentTrack;
            this.UpdatedTrack = updatedTrack;
        }

        public ITrack CurrentTrack { get; private set; }

        public ITrack UpdatedTrack { get; private set; }
    }
}
