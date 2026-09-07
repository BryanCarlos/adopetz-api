using Microsoft.EntityFrameworkCore;

namespace Adopetz.Api.Infrastructure.Persistence.Initializers
{
    public class AdopetzContextInitializer(
        AdopetzContext context)
    {
        public async Task InitializeAsync()
        {
            try
            {
                if (context.Database.IsNpgsql())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
