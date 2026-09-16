using Adopetz.Api.Application.Common.Constants;
using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Pets.Commands.Validators
{
    public class AddPetImageCommandValidator : AbstractValidator<AddPetImageCommand>
    {
        public AddPetImageCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                    .WithMessage("User ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("User ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required);

            RuleFor(x => x.PetId)
                .NotNull()
                    .WithMessage("Pet ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Pet ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required);

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
}
