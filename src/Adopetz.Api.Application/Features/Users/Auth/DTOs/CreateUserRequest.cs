namespace Adopetz.Api.Application.Features.Users.Auth.DTOs
{
    public record CreateUserRequest(
        string Name,
        string Email,
        string Phone,
        string Password
    );
}
