namespace MD.Net
{
    public interface IDiscManager
    {
        IDisc GetDisc(IDevice device);

        IResult ApplyActions(IDevice device, IActions actions, IStatus status, bool validate);
    }
}
