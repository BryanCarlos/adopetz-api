using Adopetz.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adopetz.Api.Infrastructure.Persistence
{
    public class AdopetzContext(DbContextOptions<AdopetzContext> options) : DbContext(options)
    {
        public DbSet<Pet> Pets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdopetzContext).Assembly);
        }
    }
}
