using Adopetz.Api.Domain.Common.Exceptions;

namespace Adopetz.Api.Application.Common.Exceptions
{
    public class UnauthorizedException(string message, string errorCode) : AdopetzException(message, errorCode);
}
