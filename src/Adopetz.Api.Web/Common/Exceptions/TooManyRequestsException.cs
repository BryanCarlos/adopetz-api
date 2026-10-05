using Adopetz.Api.Domain.Common.Exceptions;

namespace Adopetz.Api.Web.Common.Exceptions
{
    public class TooManyRequestsException(string message, string errorCode) : AdopetzException(message, errorCode);
}
