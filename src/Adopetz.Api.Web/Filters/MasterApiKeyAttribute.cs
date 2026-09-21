using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Adopetz.Api.Web.Filters
{
    public class MasterApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private const string ApiKeyConfigName = "MASTER_API_KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                throw new UnauthorizedException("Missing API Key in headers.", ErrorCodes.Common.UnauthorizedAccess);
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var validApiKey = configuration.GetValue<string>(ApiKeyConfigName);

            if (string.IsNullOrWhiteSpace(validApiKey) || !validApiKey.Equals(extractedApiKey))
            {
                throw new UnauthorizedException("Invalid API Key.", ErrorCodes.Common.UnauthorizedAccess);
            }

            await next();
        }
    }
}
