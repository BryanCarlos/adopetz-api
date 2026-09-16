using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Pets.Commands.Validators
{
    public class DeletePetCommandValidator : AbstractValidator<DeletePetCommand>
    {
        public DeletePetCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                    .WithMessage("User ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("User ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required);

            RuleFor(x => x.Id)
                .NotNull()
                    .WithMessage("Pet ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Pet ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required);
        }
    }
}
