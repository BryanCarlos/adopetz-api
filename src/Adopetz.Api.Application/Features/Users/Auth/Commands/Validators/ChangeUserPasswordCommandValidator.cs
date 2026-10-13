using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Users.Auth.Commands.Validators
{
    public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                    .WithMessage("User ID is required")
                    .WithErrorCode(ErrorCodes.Common.Required)
            .NotEmpty()
                .WithMessage("User ID is required")
                .WithErrorCode(ErrorCodes.Common.Required);

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                    .WithMessage("Current password is required")
                    .WithErrorCode(ErrorCodes.Common.Required);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                    .WithMessage("New password is required")
                    .WithErrorCode(ErrorCodes.Common.Required)

                .MinimumLength(8)
                    .WithMessage("The new password must be at least 8 characters long.The new password must be at least 8 characters long")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordTooShort)

                .NotEqual(x => x.CurrentPassword)
                    .WithMessage("The new password must be different from the current password")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordsMustNotMatch)

                .Matches(@"[A-Z]+")
                    .WithMessage("The new password must contain at least one capital letter")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordMustHaveUppercase)

                .Matches(@"[a-z]+")
                    .WithMessage("The new password must contain at least one lower case letter")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordMustHaveLowercase)

                .Matches(@"[0-9]+")
                    .WithMessage("The new password must contain at least one number.")
                    .WithErrorCode(ErrorCodes.User.Validation.PasswordMustHaveNumber);
        }
    }
}
