using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public record ChangeUserPasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword
    ) : IRequest;
}
