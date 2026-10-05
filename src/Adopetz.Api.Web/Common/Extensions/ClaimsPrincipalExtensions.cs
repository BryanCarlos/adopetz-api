using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Adopetz.Api.Web.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        extension (ClaimsPrincipal user)
        {
            public Guid GetUserId()
            {
                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }

                throw new UnauthorizedException("No identifier found in the token.", ErrorCodes.Common.UnauthorizedAccess);
            }
        }
    }
}
