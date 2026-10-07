namespace Adopetz.Api.Domain.Entities
{
    public class Vaccine
    {
        public Guid Id { get; private set; }
        public Guid PetId { get; private set; }
        public Pet Pet { get; private set; } = null!;
        public string Name { get; private set; } = string.Empty;
        public DateTime? ApplicationDate { get; private set; }

        private Vaccine() { }

        public static Vaccine Create(Pet pet, string name, DateTime? applicationDate)
        {
            return new Vaccine
            {
                Id = Guid.CreateVersion7(),
                Pet = pet,
                PetId = pet.Id,
                Name = name,
                ApplicationDate = applicationDate
            };
        }

        public void Update(string name, DateTime? applicationDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Adopetz.Api.Domain.Common.Exceptions.DomainException("Vaccine name is required.", Adopetz.Api.Domain.Common.Constants.ErrorCodes.Common.Required);
            if (applicationDate > DateTime.UtcNow)
                throw new Adopetz.Api.Domain.Common.Exceptions.DomainException("Vaccine date cannot be in the future.", Adopetz.Api.Domain.Common.Constants.ErrorCodes.Pet.Vaccine.InvalidVaccineDate);
            this.Name = name;
            this.ApplicationDate = applicationDate;
        }
    }
}
