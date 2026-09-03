using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Domain.Interfaces
{
    public interface IVaccineRepository
    {
        void Add(Vaccine vaccine);
        void Delete(Vaccine vaccine);
    }
}
