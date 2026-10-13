using Adopetz.Api.Domain.Common.Exceptions;

namespace Adopetz.Api.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;
        public bool RequiresPasswordChange { get; private set; } = true;
        private List<RefreshToken> _tokens { get; set; } = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> Tokens => this._tokens;

        public ICollection<Pet> Pets { get; private set; } = new List<Pet>();

        private User() { }

        public static User Create(string name, string email, string phone, string hashedPassword)
        {
            return new User
            {
                Name = name,
                Id = Guid.CreateVersion7(),
                Email = email,
                Phone = phone,
                HashedPassword = hashedPassword,
                RequiresPasswordChange = true
            };
        }

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            this._tokens.Add(refreshToken);
        }

        public void ChangePassword(string hashedPassword)
        {
            this.HashedPassword = hashedPassword;
            this.RequiresPasswordChange = false;
        }

        public bool IsRefreshTokenValid(string refreshToken, DateTime now)
        {
            var token = this._tokens.FirstOrDefault(t => t.Token == refreshToken);
            return token?.ExpiresAt > now;
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            var token = this._tokens.FirstOrDefault(t => t.Token == refreshToken);
            if (token != null)
            {
                this._tokens.Remove(token);
            }
        }
    }
}
