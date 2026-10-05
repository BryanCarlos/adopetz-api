using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class UserExtensions
    {
        extension (User user)
        {
            public UserResponse ToResponse()
            {
                return new UserResponse(
                    Email: user.Email,
                    Phone: user.Phone,
                    Name: user.Name
                );
            }
        }
    }
}
