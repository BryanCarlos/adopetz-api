using Adopetz.Api.Application.Features.Users.Auth.DTOs;

namespace Adopetz.Api.Application.Features.Users.Auth
{
    public interface IUserAuthService
    {
        Task<UserTokensResponse> LoginAsync(UserLoginRequest request, DateTime now, CancellationToken cancellationToken);
        
        Task RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken);
    }
}
