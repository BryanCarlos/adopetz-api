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

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await context.Users.AnyAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Users.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.Token == refreshToken), cancellationToken);
        }
    }
}

