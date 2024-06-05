namespace MD.Net
{
    public interface ICapacity
    {
        int PercentUsed { get; }

        int PercentFree { get; }
    }
}
