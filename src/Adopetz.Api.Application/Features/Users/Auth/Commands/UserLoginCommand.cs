using Adopetz.Api.Application.Features.Users.Auth.DTOs;
using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public record UserLoginCommand(
        DateTime Now,
        string Email,
        string Password
    ) : IRequest<UserTokensResponse>;
}
