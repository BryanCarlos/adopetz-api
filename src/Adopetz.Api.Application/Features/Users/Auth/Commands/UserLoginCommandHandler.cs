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
    public class UserLoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IHasher hasher,
        IUnitOfWork uow) : IRequestHandler<UserLoginCommand, UserTokensResponse>
    {
        public async Task<UserTokensResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null || !passwordHasher.Verify(request.Password, user.HashedPassword))
            {
                throw new UnauthorizedException("Invalid credentials.", ErrorCodes.Common.Security.InvalidCredentials);
            }

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
