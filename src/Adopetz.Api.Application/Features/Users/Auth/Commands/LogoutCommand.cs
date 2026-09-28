using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public record LogoutCommand(
        Guid UserId,
        string RefreshToken
    ) : IRequest;
}
