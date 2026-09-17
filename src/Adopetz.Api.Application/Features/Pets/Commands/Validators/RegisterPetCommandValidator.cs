using Adopetz.Api.Application.Common.Constants;
using Adopetz.Api.Application.Features.Pets.Commands.DTOs;
using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Pets.Commands.Validators
{
    public class RegisterPetCommandValidator : AbstractValidator<RegisterPetCommand>
    {
        public RegisterPetCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                    .WithMessage("Name is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Name is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .MaximumLength(100)
                    .WithMessage("The name must be a maximum of 100 characters")
                    .WithErrorCode(ErrorCodes.Common.MaximumLength);

            RuleFor(x => x.Breed)
                .MaximumLength(100)
                    .WithMessage("The breed must be a maximum of 100 characters")
                    .WithErrorCode(ErrorCodes.Common.MaximumLength);

            RuleFor(x => x.About)
                .NotNull()
                    .WithMessage("About is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("About is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .MaximumLength(500)
                    .WithMessage("The about must be a maximum of 500 characters")
                    .WithErrorCode(ErrorCodes.Common.MaximumLength);

            RuleFor(x => x.UserId)
                .NotNull()
                    .WithMessage("User ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("User ID required")
                    .WithErrorCode(ErrorCodes.Common.Required);

            RuleFor(x => x.AgeInYears)
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Age cannot be negative")
                    .WithErrorCode(ErrorCodes.Common.InvalidRange)
                .GreaterThanOrEqualTo(100)
                    .WithMessage("Age looks unreal for a pet")
                    .WithErrorCode(ErrorCodes.Common.InvalidRange);

            RuleFor(x => x.PetImages)
                .Must(i => i.Count <= 4)
                    .WithMessage("Maximum 4 images per pet")
                    .WithErrorCode(ErrorCodes.Pet.Image.MaxImagesLimit);

            RuleForEach(x => x.PetImages)
                .SetValidator(new CreatePetImageRequestValidator());

            RuleForEach(x => x.Vaccines)
                .SetValidator(new CreateVaccineRequestValidator());
        }
    }

    public class CreatePetImageRequestValidator : AbstractValidator<CreatePetImageRequest>
    {
        public CreatePetImageRequestValidator()
        {
            RuleFor(x => x.ContentType)
                .NotNull()
                    .WithMessage("Content type is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Content type is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .Must(type => ImageConstants.AllowedContentTypes.Contains(type))
                    .WithMessage("Content type is invalid. Use jpeg, png, webp or avif")
                    .WithErrorCode(ErrorCodes.Pet.Image.InvalidFormat);

            RuleFor(x => x.Base64Data)
                .NotNull()
                    .WithMessage("Base data is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Base data is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .Matches(@"^[a-zA-Z0-9\+/]*={0,2}$")
                    .WithMessage("Invalid Base64 format.")
                    .WithErrorCode(ErrorCodes.Pet.Image.InvalidFormat)
                .Must(BeAValidBase64String)
                    .WithMessage("Invalid Base64 format.")
                    .WithErrorCode(ErrorCodes.Pet.Image.InvalidFormat);
        }

        private bool BeAValidBase64String(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                return false;
            }

            Span<byte> buffer = new Span<byte>(new byte[base64String.Length]);
            return Convert.TryFromBase64String(base64String, buffer, out int bytesParsed);
        }
    }

    public class CreateVaccineRequestValidator : AbstractValidator<CreateVaccineRequest>
    {
        public CreateVaccineRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                    .WithMessage("Name is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Name is required")
                    .WithErrorCode(ErrorCodes.Common.Required);
        }
    }
}
