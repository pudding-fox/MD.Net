namespace MD.Net
{
    public interface IDiscAction : IAction
    {
        IDisc CurrentDisc { get; }

        IDisc UpdatedDisc { get; }
    }
}
