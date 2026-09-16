using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Pets.Commands.Validators
{
    public class UpdateVaccineCommandHandler : AbstractValidator<UpdateVaccineCommand>
    {
        public UpdateVaccineCommandHandler()
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

            RuleFor(x => x.VaccineId)
                .NotNull()
                    .WithMessage("Vaccine ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Vaccine ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required);

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
