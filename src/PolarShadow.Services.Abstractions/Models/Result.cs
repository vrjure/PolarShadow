namespace PolarShadow.Services
{
    public class Result
    {
        public static Result Success = new Result(ResultCode.Success, nameof(ResultCode.Success));

        public Result() { }
        public Result(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ResultObject : Result
    {
        public ResultObject(object data):this(ResultCode.Success, nameof(ResultCode.Success), data)
        {
            
        }

        public ResultObject(int code, string message, object data) :base(code, message) 
        {
            Data = data;
        }
        public object Data { get; set; }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }
    }

    public sealed class ResultCode
    {
        public const int Success = 0;
        public const int ParameterError = 1000;
        public const int UsernameOrPasswordError = 1001;
        public const int ServerConfigError = 9998;
        public const int UnhandledException = 9999;
    }
}
