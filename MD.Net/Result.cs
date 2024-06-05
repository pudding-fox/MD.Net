using MD.Net.Resources;

namespace MD.Net
{
    public class Result : IResult
    {
        public Result(string message, ResultStatus status)
        {
            if (string.IsNullOrEmpty(message))
            {
                switch (status)
                {
                    case ResultStatus.Success:
                        message = Strings.Result_Success;
                        break;
                    case ResultStatus.Failure:
                        message = Strings.Result_UnknownError;
                        break;
                }
            }
            this.Message = message;
            this.Status = status;
        }

        public string Message { get; private set; }

        public ResultStatus Status { get; private set; }

        public static IResult Success
        {
            get
            {
                return new Result(string.Empty, ResultStatus.Success);
            }
        }

        public static IResult Failure(string message)
        {
            return new Result(message, ResultStatus.Failure);
        }
    }
}
