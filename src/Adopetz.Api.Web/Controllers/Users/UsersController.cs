using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Features.Users.Auth.Commands;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Web.Common.Constants;
using Adopetz.Api.Web.Common.Extensions;
using Adopetz.Api.Web.Filters;
using Adopetz.Api.Web.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Adopetz.Api.Web.Controllers.Users
{
    [Route("users")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";

        [MasterApiKey]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
        {
            await mediator.Send(command);
            return Created();
        }

        [AllowAnonymous]
        [HttpPost("auth/login")]
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
        [HttpPost("auth/refresh-token")]
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

        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var command = new LogoutCommand(
                    UserId: User.GetUserId(),
                    RefreshToken: refreshToken
                );

                await mediator.Send(command);

                Response.Cookies.Delete(RefreshTokenCookieName);
            }

            return NoContent();
        }
    }
}
