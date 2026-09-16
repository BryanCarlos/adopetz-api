using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                    .WithMessage("Name is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Name is required")
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

            RuleFor(x => x.Phone)
                .NotNull()
                    .WithMessage("Email is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .NotEmpty()
                    .WithMessage("Email is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
                .MaximumLength(30)
                    .WithMessage("Phone number too long")
                    .WithErrorCode(ErrorCodes.Common.MaximumLength);

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage("Password is required")
                    .WithErrorCode(ErrorCodes.Common.Required)

                .MinimumLength(8)
                    .WithMessage("The password must be at least 8 characters long.The password must be at least 8 characters long")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordTooShort)

                .Matches(@"[A-Z]+")
                    .WithMessage("The password must contain at least one capital letter")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordMustHaveUppercase)

                .Matches(@"[a-z]+")
                    .WithMessage("The password must contain at least one lower case letter")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordMustHaveLowercase)

                .Matches(@"[0-9]+")
                    .WithMessage("The password must contain at least one number.")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordMustHaveNumber);
        }
    }
}
