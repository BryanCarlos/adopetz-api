using Adopetz.Api.Domain.Common.Constants;
using FluentValidation;

namespace Adopetz.Api.Application.Features.Pets.Queries.Validators
{
    public class SearchPublicPetsQueryValidator : AbstractValidator<SearchPublicPetsQuery>
    {
        public SearchPublicPetsQueryValidator()
        {
            When(x => x.Filter != null, () =>
            {
                RuleFor(x => x.Filter.SearchTerm)
                    .MaximumLength(100)
                        .WithMessage("The search term cannot exceed 100 characters")
                        .WithErrorCode(ErrorCodes.Common.MaximumLength);

                RuleFor(x => x.Filter.MinAge)
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("The minimum age cannot be negative")
                        .WithErrorCode(ErrorCodes.Common.InvalidRange);

                RuleFor(x => x.Filter.MaxAge)
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("Maximum age cannot be negative")
                        .WithErrorCode(ErrorCodes.Common.InvalidRange)
                    .GreaterThanOrEqualTo(x => x.Filter.MinAge).When(x => x.Filter.MinAge.HasValue)
                        .WithMessage("The maximum age must be greater than or equal to the minimum age")
                        .WithErrorCode(ErrorCodes.Common.InvalidRange);

                RuleFor(x => x.Filter.AdoptionStatus)
                    .IsInEnum()
                        .WithMessage("Invalid adoption status")
                        .WithErrorCode(ErrorCodes.Common.InvalidValue);
            });

            When(x => x.Pagination != null, () =>
            {
                RuleFor(x => x.Pagination.PageNumber)
                    .GreaterThanOrEqualTo(1)
                        .WithMessage("Page number must be greater than or equal to 1.")
                        .WithErrorCode(ErrorCodes.Common.InvalidRange);

                RuleFor(x => x.Pagination.PageSize)
                    .GreaterThanOrEqualTo(1)
                        .WithMessage("Page size must be at least 1")
                        .WithErrorCode(ErrorCodes.Common.InvalidRange)
                    .LessThanOrEqualTo(50)
                        .WithMessage("To avoid overload, keep maximum 50 pets at a time")
                        .WithErrorCode(ErrorCodes.Common.InvalidRange);
            });
        }
    }
}
