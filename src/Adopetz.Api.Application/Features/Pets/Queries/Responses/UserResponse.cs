namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record UserResponse(
        Guid Id,
        string Email,
        string Phone,
        string Name
    );
}
