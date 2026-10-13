using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Domain.Interfaces
{
    public interface IRefreshToken
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<ICollection<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        void Add(RefreshToken refreshToken);
        void Delete(RefreshToken refreshToken);
    }
}
