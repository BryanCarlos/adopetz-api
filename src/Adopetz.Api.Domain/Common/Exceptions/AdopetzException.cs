namespace Adopetz.Api.Domain.Common.Exceptions
{
    public abstract class AdopetzException(string message, string errorCode) : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
    }
}
