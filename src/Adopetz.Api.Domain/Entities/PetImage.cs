namespace Adopetz.Api.Domain.Entities
{
    public class PetImage
    {
        public Guid Id { get; private set; }
        public Guid PetId { get; private set; }
        public Pet Pet { get; private set; } = null!;
        public string FilePath { get; private set; } = string.Empty;
    }
}
