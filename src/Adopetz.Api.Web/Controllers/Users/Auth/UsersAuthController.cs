using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Features.Users.Auth.Commands;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Web.Common.Constants;
using Adopetz.Api.Web.Common.Extensions;
using Adopetz.Api.Web.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Adopetz.Api.Web.Controllers.Users.Auth
{
    [Route("users/auth")]
    [ApiController]
    public class UsersAuthController(IMediator mediator) : ControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";

        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting(RateLimitingPolicies.LoginStrict)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var command = new UserLoginCommand(
                Now: DateTime.UtcNow,
                Email: request.Email,
                Password: request.Password
            );

            var response = await mediator.Send(command);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = response.RefreshTokenExpirationDate,
                IsEssential = true
            };

            Response.Cookies.Append(RefreshTokenCookieName, response.RefreshToken, cookieOptions);

            return Ok(new { AccessToken = response.AccessToken });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [EnableRateLimiting(RateLimitingPolicies.LoginStrict)]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var currentRefreshToken))
            {
                throw new UnauthorizedException("Expired session. Login again.", ErrorCodes.Common.UnauthorizedAccess);
            }

            var command = new RefreshTokenCommand(DateTime.UtcNow, currentRefreshToken);

            var response = await mediator.Send(command);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = response.RefreshTokenExpirationDate,
                IsEssential = true
            };

            Response.Cookies.Append(RefreshTokenCookieName, response.RefreshToken, cookieOptions);

            return Ok(new { AccessToken = response.AccessToken });
        }

        [HttpPatch("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var command = new ChangeUserPasswordCommand(
                UserId: User.GetUserId(),
                CurrentPassword: request.CurrentPassword,
                NewPassword: request.NewPassword
            );

            await mediator.Send(command);

            return NoContent();
        }
    }
}
