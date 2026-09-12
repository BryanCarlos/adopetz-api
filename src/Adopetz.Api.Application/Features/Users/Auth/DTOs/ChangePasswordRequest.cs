namespace Adopetz.Api.Application.Features.Users.Auth.DTOs
{
    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );
}
