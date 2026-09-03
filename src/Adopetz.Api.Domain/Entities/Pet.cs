namespace Adopetz.Api.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Breed { get;private set; }
        public DateTime? RescueDate { get; private set; }
        public bool Adopted { get; private set; }

        public ICollection<Vaccine> Vaccines { get; private set; } = new List<Vaccine>();
        public ICollection<PetImage> Images { get; private set; } = new List<PetImage>();
    }
}
