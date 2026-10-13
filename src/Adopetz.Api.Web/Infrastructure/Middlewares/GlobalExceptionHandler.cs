using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adopetz.Api.Web.Infrastructure.Middlewares
{
    public class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService
        ) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "Internal Server Error";
            var detail = "An unexpected error occurred.";
            var errorCode = "INTERNAL_SERVER_ERROR";
            IDictionary<string, object>? validationErrors = null;
            var extensions = new Dictionary<string, object?>();

            switch (exception)
            {
                case AdopetzException adopetzException:
                    logger.LogWarning("Business rule violated: {Message} | Type: {Type}", exception.Message, exception.GetType().Name);

                    statusCode = adopetzException switch
                    {
                        ConflictException => StatusCodes.Status400BadRequest,
                        UnauthorizedException => StatusCodes.Status401Unauthorized,
                        ForbiddenAccessException => StatusCodes.Status403Forbidden,
                        PasswordChangeRequiredException => StatusCodes.Status403Forbidden,
                        NotFoundException => StatusCodes.Status404NotFound,
                        DomainException => StatusCodes.Status422UnprocessableEntity,
                        ExternalServiceException => StatusCodes.Status502BadGateway,
                        _ => StatusCodes.Status400BadRequest
                    };

                    title = adopetzException switch
                    {
                        ConflictException => "A conflict occurred",
                        UnauthorizedException => "Unauthorized access",
                        ForbiddenAccessException => "Forbidden access",
                        PasswordChangeRequiredException => "Password change required",
                        NotFoundException => "Resource not found",
                        DomainException => "Unprocessable entity",
                        ExternalServiceException => "External service is down",
                        _ => "Unexpected error"
                    };

                    errorCode = adopetzException.ErrorCode;
                    detail = adopetzException.Message;
                    break;

                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Validation Failure";
                    detail = "One or more validation errors occurred.";
                    errorCode = ErrorCodes.Common.ValidationFailure;

                    validationErrors = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => (object)g.Select(e => new
                            {
                                Code = e.ErrorCode,
                                Message = e.ErrorMessage
                            }).ToArray()
                        );
                    break;

                default:
                    logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
                    break;
            }

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            extensions.Add("traceId", traceId);
            extensions.Add("requestId", httpContext.TraceIdentifier);
            extensions.Add("errorCode", errorCode);

            if (validationErrors != null)
            {
                extensions.Add("validationErrors", validationErrors);
            }

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                    Extensions = extensions
                }
            });
        }
    }
}
