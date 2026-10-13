using Adopetz.Api.Domain.Common.Interfaces;

namespace Adopetz.Api.Infrastructure.Persistence.Utils
{
    public class UnitOfWork(AdopetzContext context) : IUnitOfWork
    {
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
