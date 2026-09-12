using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Application.Features.Users.Auth.DTOs;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;

namespace Adopetz.Api.Application.Features.Users.Auth
{
    public class UserAuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IUnitOfWork uow) : IUserAuthService
    {
        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null || !passwordHasher.Verify(request.CurrentPassword, user.HashedPassword))
            {
                throw new UnauthorizedException("Invalid credentials.", ErrorCodes.Common.Security.InvalidCredentials);
            }

            var newHashedPassword = passwordHasher.Hash(request.NewPassword);

            user.ChangePassword(newHashedPassword);
            await uow.CommitAsync(cancellationToken);
        }

        public async Task<UserTokensResponse> LoginAsync(UserLoginRequest request, DateTime now, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null || !passwordHasher.Verify(request.Password, user.HashedPassword))
            {
                throw new UnauthorizedException("Invalid credentials.", ErrorCodes.Common.Security.InvalidCredentials);
            }

            var accessToken = tokenGenerator.GenerateAccessToken(user, now);
            var refreshToken = tokenGenerator.GenerateRefreshToken(now);

            var userRefreshToken = RefreshToken.Create(
                user: user,
                token: refreshToken.Token,
                expiresAt: refreshToken.ExpiresAt
            );

            user.AddRefreshToken(userRefreshToken);

            await uow.CommitAsync(cancellationToken);

            return new UserTokensResponse(
                AccessToken: accessToken.Token,
                RefreshToken: refreshToken.Token
            );
        }

        public async Task RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var emailRegistered = await userRepository.EmailExistsAsync(request.Email, cancellationToken);

            if (emailRegistered)
            {
                throw new ConflictException("Email already in use.", ErrorCodes.User.EmailAlreadyInUse);
            }

            var hashedPassword = passwordHasher.Hash(request.Password);

            var newUser = User.Create(
                email: request.Email,
                hashedPassword: hashedPassword
            );

            userRepository.Add(newUser);
            await uow.CommitAsync(cancellationToken);
        }
    }
}
