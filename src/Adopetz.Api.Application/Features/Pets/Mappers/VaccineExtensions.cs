using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class VaccineExtensions
    {
        extension (Vaccine vaccine)
        {
            public PublicVaccineResponse ToResponse()
            {
                return new PublicVaccineResponse(
                    Name: vaccine.Name,
                    ApplicationDate: vaccine.ApplicationDate
                );
            }

            public ManagerVaccineResponse ToManagerResponse()
            {
                return new ManagerVaccineResponse(
                    Id: vaccine.Id,
                    Name: vaccine.Name,
                    ApplicationDate: vaccine.ApplicationDate
                );
            }
        }
    }
}
