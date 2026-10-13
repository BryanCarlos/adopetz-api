using Adopetz.Api.Domain.Common.Exceptions;

namespace Adopetz.Api.Application.Common.Exceptions
{
    public class NotFoundException(string message, string errorCode) : AdopetzException(message, errorCode);
}
