namespace Adopetz.Api.Domain.Common.Exceptions
{
    public class DomainException(string message, string errorCode) : AdopetzException(message, errorCode);
}
