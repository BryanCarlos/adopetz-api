using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Exceptions;
using static Adopetz.Api.Domain.Common.Constants.ErrorCodes.Pet;

namespace Adopetz.Api.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Breed { get; private set; } = null;
        public string About { get; private set; } = string.Empty;
        public DateTime? RescueDate { get; private set; } = null;
        public bool Adopted { get; private set; } = false;
        public int? AgeInYears { get; private set; } = null;
        public Guid UserId { get; private set; }

        public User User { get; private set; } = null!;

        private List<Vaccine> _vaccines { get; set; } = new List<Vaccine>();
        public IReadOnlyList<Vaccine> Vaccines => this._vaccines;

        private List<PetImage> _images { get; set; } = new List<PetImage>();
        public IReadOnlyList<PetImage> Images => this._images;

        private Pet() { }

        public static Pet Create(string name, string? breed, string about, DateTime? rescueDate, Guid userId, int? ageInYears)
        {
            return new Pet
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                Breed = breed,
                About = about,
                RescueDate = rescueDate,
                UserId = userId,
                AgeInYears = ageInYears
            };
        }

        public void Update(string name, bool adopted, string? breed, string about, DateTime? rescueDate, int? ageInYears)
        {
            Name = name;
            Adopted = adopted;
            Breed = breed;
            About = about;
            RescueDate = rescueDate;
            AgeInYears = ageInYears;
        }


        public void AddVaccine(string name, DateTime? applicationDate)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Vaccine name is required.", ErrorCodes.Common.Required);
            }

            if (applicationDate > DateTime.UtcNow)
            {
                throw new DomainException("Vaccine date cannot be in the future.", ErrorCodes.Pet.Vaccine.InvalidVaccineDate);
            }

            var newVaccine = Vaccine.Create(this, name, applicationDate);

            _vaccines.Add(newVaccine);
        }

        public void AddImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new DomainException("File path is required.", ErrorCodes.Common.Required);
            }

            if (_images.Count >= 4)
            {
                throw new DomainException("O limite máximo é de 4 imagens.", ErrorCodes.Pet.Image.MaxImagesLimit);
            }

            var image = PetImage.Create(this, filePath);

            _images.Add(image);
        }

        public void RemoveVaccine(Guid vaccineId)
        {
            var vaccine = _vaccines.FirstOrDefault(v => v.Id == vaccineId);

            if (vaccine == null)
            {
                throw new DomainException("The reported vaccine does not belong to this pet or does not exist.", ErrorCodes.Pet.Vaccine.NotFound);
            }

            _vaccines.Remove(vaccine);
        }

        public string RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(v => v.Id == imageId);

            if (image == null)
            {
                throw new DomainException("The reported image does not belong to this pet or does not exist.", ErrorCodes.Pet.Image.NotFound);
            }

            _images.Remove(image);

            return image.FilePath;
        }

        public void UpdateVaccine(Guid vaccineId, string name, DateTime? applicationDate)
        {
            var vaccine = _vaccines.FirstOrDefault(v => v.Id == vaccineId);

            if (vaccine == null)
            {
                throw new DomainException("The reported vaccine does not belong to this pet or does not exist.", ErrorCodes.Pet.Vaccine.NotFound);
            }

            vaccine.Update(name, applicationDate);
        }
    }
}
