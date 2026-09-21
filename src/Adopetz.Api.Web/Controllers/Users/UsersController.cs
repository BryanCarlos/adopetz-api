using Adopetz.Api.Application.Features.Users.Auth.Commands;
using Adopetz.Api.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adopetz.Api.Web.Controllers.Users
{
    [Route("users")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [MasterApiKey]
        [AllowAnonymous]
        [HttpPost("user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
        {
            await mediator.Send(command);
            return Created();
        }
    }
}
