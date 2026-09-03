namespace Adopetz.Api.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;
        public ICollection<RefreshToken> Tokens { get; set; } = new List<RefreshToken>();
    }
}
