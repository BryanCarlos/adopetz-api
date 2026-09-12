namespace Adopetz.Api.Application.Features.Users.Auth.DTOs
{
    public record CreateUserRequest(
        string Email,
        string Password
    );
}
