using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Application.Features.Users.Auth.DTOs;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands
{
    public class RefreshTokenCommandHandler(
            IUserRepository userRepository,
            ITokenGenerator tokenGenerator,
            IHasher hasher,
            IUnitOfWork uow
        ) : IRequestHandler<RefreshTokenCommand, UserTokensResponse>
    {
        public async Task<UserTokensResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var hashedUserRefreshToken = hasher.Hash(request.RefreshToken);
            var user = await userRepository.GetByRefreshTokenAsync(hashedUserRefreshToken, cancellationToken);

            if (user == null || !user.IsRefreshTokenValid(hashedUserRefreshToken, request.Now))
            {
                throw new UnauthorizedException("Invalid token", ErrorCodes.User.InvalidToken);
            }

            user.RemoveRefreshToken(hashedUserRefreshToken);

            var accessToken = tokenGenerator.GenerateAccessToken(user, request.Now);
            var refreshToken = tokenGenerator.GenerateRefreshToken(request.Now);

            var hashedRefreshToken = hasher.Hash(refreshToken.Token);

            var userRefreshToken = RefreshToken.Create(
                user: user,
                token: hashedRefreshToken,
                expiresAt: refreshToken.ExpiresAt
            );

            user.AddRefreshToken(userRefreshToken);

            await uow.CommitAsync(cancellationToken);

            return new UserTokensResponse(
                AccessToken: accessToken.Token,
                RefreshToken: refreshToken.Token,
                RefreshTokenExpirationDate: refreshToken.ExpiresAt
            );
        }
    }
}
