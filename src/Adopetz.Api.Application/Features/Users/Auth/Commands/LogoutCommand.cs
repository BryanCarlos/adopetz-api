using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public record LogoutCommand(
        string RefreshToken
    ) : IRequest;
}
