namespace Adopetz.Api.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string Token { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; private set; }

        private RefreshToken() { }

        public static RefreshToken Create(User user, string token, DateTime expiresAt)
        {
            return new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                User = user,
                UserId = user.Id,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}
