using Core.DTOs.Request.Maintenance;
using FluentValidation;

namespace Core.Validators;

public class MaintenanceSearchRequestValidator : AbstractValidator<MaintenanceSearchRequest>
{
    public MaintenanceSearchRequestValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).When(x => x.Month.HasValue)
            .WithMessage("Month must be between 1 and 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, DateTime.UtcNow.Year + 1).When(x => x.Year.HasValue)
            .WithMessage($"Year must be between 2000 and {DateTime.UtcNow.Year + 1}.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
