using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Adopetz.Api.Infrastructure.Persistence.Repositories
{
    internal class UserRepository(AdopetzContext context) : IUserRepository
    {
        public void Add(User user)
        {
            context.Users.Add(user);
        }

        public void Delete(User user)
        {
            context.Users.Remove(user);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
