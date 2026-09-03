namespace Adopetz.Api.Domain.Entities
{
    public class Vaccine
    {
        public Guid Id { get; private set; }
        public Guid PetId { get; private set; }
        public Pet Pet { get; private set; } = null!;
        public string VaccineName { get; private set; } = string.Empty;
        public DateTime? ApplicationDate { get; private set; }
    }
}
