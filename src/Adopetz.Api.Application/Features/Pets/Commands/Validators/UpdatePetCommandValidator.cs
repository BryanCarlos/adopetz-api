using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Pets.Commands.Validators
{
    public class UpdatePetCommandValidator : AbstractValidator<UpdatePetCommand>
    {
        public UpdatePetCommandValidator()
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

            RuleFor(x => x.Id)
                .NotNull()
                    .WithMessage("Pet ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Pet ID required")
                    .WithErrorCode(ErrorCodes.Common.Required);

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
        }
    }
}
