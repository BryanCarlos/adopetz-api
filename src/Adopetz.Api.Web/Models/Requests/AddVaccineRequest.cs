namespace Adopetz.Api.Web.Models.Requests
{
    public record AddVaccineRequest(
        string Name,
        DateTime? ApplicationDate
    );
}
