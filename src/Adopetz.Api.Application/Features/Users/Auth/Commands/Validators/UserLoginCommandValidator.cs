using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands.Validators
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator()
        {
            RuleFor(x => x.Now)
                .NotNull()
                    .WithMessage("Current date is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Current date is required")
                    .WithErrorCode(ErrorCodes.Common.Required);

            RuleFor(x => x.Email)
                .NotNull()
                    .WithMessage("Email is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Email is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .EmailAddress()
                    .WithMessage("Invalid email format")
                    .WithErrorCode(ErrorCodes.Common.InvalidValue);

            RuleFor(x => x.Password)
                .NotNull()
                    .WithMessage("Password is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Password is required")
                    .WithErrorCode(ErrorCodes.Common.Required);
        }
    }
}
