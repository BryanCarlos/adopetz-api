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
            this.Name = name;
            this.ApplicationDate = applicationDate;
        }
    }
}
