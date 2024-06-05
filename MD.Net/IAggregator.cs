namespace MD.Net
{
    public interface IAggregator<T>
    {
        void Append(T value);

        T Aggregate();
    }
}
