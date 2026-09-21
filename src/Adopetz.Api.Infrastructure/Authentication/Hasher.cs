using Adopetz.Api.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Adopetz.Api.Infrastructure.Authentication
{
    public class Hasher : IHasher
    {
        public string Hash(string value)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(bytes);
        }
    }
}
