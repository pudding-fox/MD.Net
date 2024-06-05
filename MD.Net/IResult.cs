namespace MD.Net
{
    public interface IResult
    {
        string Message { get; }

        ResultStatus Status { get; }
    }

    public enum ResultStatus
    {
        None,
        Success,
        Partial,
        Failure
    }
}
