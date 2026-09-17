using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public record RegisterUserCommand(
        string Name,
        string Email,
        string Phone,
        string Password
    ) : IRequest;
}
