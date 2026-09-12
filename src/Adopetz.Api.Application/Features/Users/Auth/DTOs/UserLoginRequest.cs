namespace Adopetz.Api.Application.Features.Users.Auth.DTOs
{
    public record UserLoginRequest(
        string Email,
        string Password
    );
}
