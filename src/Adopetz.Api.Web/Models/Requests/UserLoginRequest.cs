namespace Adopetz.Api.Web.Models.Requests
{
    public record UserLoginRequest(
        string Email,
        string Password
    );
}
