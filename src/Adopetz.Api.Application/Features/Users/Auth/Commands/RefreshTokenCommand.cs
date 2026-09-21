using Adopetz.Api.Application.Features.Users.Auth.DTOs;
using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public record RefreshTokenCommand(
        DateTime Now,
        string RefreshToken
    ) : IRequest<UserTokensResponse>;
}
