namespace Adopetz.Api.Web.Models.Requests
{
    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );
}
