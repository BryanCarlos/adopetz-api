using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class VaccineExtensions
    {
        extension (Vaccine vaccine)
        {
            public VaccineResponse ToResponse()
            {
                return new VaccineResponse(
                    Id: vaccine.Id,
                    Name: vaccine.Name,
                    ApplicationDate: vaccine.ApplicationDate
                );
            }
        }
    }
}
