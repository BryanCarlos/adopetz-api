using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Domain.Interfaces
{
    public interface IPetImageRepository
    {
        void Add(PetImage petImage);
        void Delete(PetImage petImage);
        void AddRange(IEnumerable<PetImage> petImages);
    }
}
