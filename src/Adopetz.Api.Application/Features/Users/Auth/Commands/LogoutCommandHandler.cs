using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public class LogoutCommandHandler(
        IUserRepository userRepository,
        IHasher hasher,
        IUnitOfWork uow) : IRequestHandler<LogoutCommand>
    {
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return;
            }

            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
            var hashedToken = hasher.Hash(request.RefreshToken);

            var tokenToRemove = user?.Tokens.FirstOrDefault(t => t.Token == hashedToken);

            if (tokenToRemove == null)
            {
                return;
            }

            user!.RemoveRefreshToken(tokenToRemove.Token);
            await uow.CommitAsync(cancellationToken);
        }
    }
}
