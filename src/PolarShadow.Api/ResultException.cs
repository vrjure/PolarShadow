namespace PolarShadow.Api
{
    public class ResultException : Exception
    {
        public ResultException(int code, string message) : base(message)
        {
            this.Code = code;
        }
        public int Code { get; }
    }
}
