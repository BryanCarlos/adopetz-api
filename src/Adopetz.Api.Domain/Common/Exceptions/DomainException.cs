namespace Adopetz.Api.Domain.Common.Exceptions
{
    public abstract class DomainException(string message, string errorCode) : AdopetzException(message, errorCode);
}
