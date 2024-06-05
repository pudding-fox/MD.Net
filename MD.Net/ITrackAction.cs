namespace MD.Net
{
    public interface ITrackAction : IDiscAction
    {
        ITrack CurrentTrack { get; }

        ITrack UpdatedTrack { get; }
    }
}
