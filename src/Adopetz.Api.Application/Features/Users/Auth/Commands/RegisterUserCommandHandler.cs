using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork uow) : IRequestHandler<RegisterUserCommand>
    {
        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var emailRegistered = await userRepository.EmailExistsAsync(request.Email, cancellationToken);

            if (emailRegistered)
            {
                throw new ConflictException("Email already in use.", ErrorCodes.User.EmailAlreadyInUse);
            }

            var hashedPassword = passwordHasher.Hash(request.Password);

            var newUser = User.Create(
                name: request.Name,
                email: request.Email,
                phone: request.Phone,
                hashedPassword: hashedPassword
            );

            userRepository.Add(newUser);
            await uow.CommitAsync(cancellationToken);
        }
    }
}
