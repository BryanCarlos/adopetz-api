using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public class ChangeUserPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork uow) : IRequestHandler<ChangeUserPasswordCommand>
    {
        public async Task Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null || !passwordHasher.Verify(request.CurrentPassword, user.HashedPassword))
            {
                throw new UnauthorizedException("Invalid credentials.", ErrorCodes.Common.Security.InvalidCredentials);
            }

            var newHashedPassword = passwordHasher.Hash(request.NewPassword);

            user.ChangePassword(newHashedPassword);
            await uow.CommitAsync(cancellationToken);
        }
    }
}
