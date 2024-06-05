namespace MD.Net
{
    public interface IFormatManager
    {
        string Convert(string fileName, Compression compression, IStatus status);
    }
}
