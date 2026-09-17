using Adopetz.Api.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Adopetz.Api.Infrastructure.Authentication
{
    internal class PasswordHasher : IPasswordHasher
    {
        private readonly IPasswordHasher<object> _passwordHasher = new PasswordHasher<object>();
        public string Hash(string password)
        {
            return this._passwordHasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string hashedPassword)
        {
            var result = this._passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
