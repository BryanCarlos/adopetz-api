namespace Adopetz.Api.Application.Features.Users.Auth.DTOs
{
    public record UserTokensResponse(
        string AccessToken,
        string RefreshToken
    );
}
