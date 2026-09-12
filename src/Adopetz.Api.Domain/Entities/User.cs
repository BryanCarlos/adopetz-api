namespace Adopetz.Api.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;
        public bool RequiresPasswordChange { get; private set; } = true;
        private List<RefreshToken> _tokens { get; set; } = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> Tokens => this._tokens.AsReadOnly();

        public ICollection<Pet> Pets { get; private set; } = new List<Pet>();

        private User() { }

        public static User Create(string email, string hashedPassword)
        {
            return new User
            {
                Id = Guid.CreateVersion7(),
                Email = email,
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
    }
}
