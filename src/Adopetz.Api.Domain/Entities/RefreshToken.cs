namespace Adopetz.Api.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string Token { get; private set; } = string.Empty;
        public DateTime GeneratedAt { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool Revoked { get; private set; }
    }
}
