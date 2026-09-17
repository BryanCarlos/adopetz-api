using Adopetz.Api.Domain.Common.Exceptions;

namespace Adopetz.Api.Application.Common.Exceptions
{
    public class ConflictException(string message, string errorCode) : AdopetzException(message, errorCode);
}
