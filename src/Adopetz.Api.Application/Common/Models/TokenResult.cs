namespace Adopetz.Api.Application.Common.Models
{
    public record TokenResult(
        string Token, DateTime ExpiresAt
    );
}
