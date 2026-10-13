using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;

namespace Adopetz.Api.Web.Infrastructure.Middlewares
{
    public class ChangePasswordMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await next(context);
                return;
            }

            var changeNeededClaim = context.User.FindFirst("requiresPasswordChange")?.Value;
            var isChangeNeeded = changeNeededClaim == "true";

            if (isChangeNeeded)
            {
                var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

                var isAllowedRoute =
                    path.EndsWith("auth/login") ||
                    path.EndsWith("auth/refresh-token") ||
                    path.EndsWith("me/change-password") ||
                    path.EndsWith("auth/logout");

                if (!isAllowedRoute)
                {
                    throw new PasswordChangeRequiredException("Password change is required before proceeding.", ErrorCodes.User.Security.PasswordChangeRequired);
                }
            }

            await next(context);
        }
    }
}
