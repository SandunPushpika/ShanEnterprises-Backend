using Core.DTOs.Request.Maintenance;
using FluentValidation;

namespace Core.Validators;

public class MaintenanceCreateRequestValidator : AbstractValidator<MaintenanceCreateRequest>
{
    public MaintenanceCreateRequestValidator()
    {
        RuleFor(x => x.VehicleId)
            .GreaterThan(0).WithMessage("A valid vehicle must be selected.");

        RuleFor(x => x.MaintenanceStart)
            .NotEmpty().WithMessage("Maintenance start date is required.");

        RuleFor(x => x.MaintenanceEnd)
            .NotEmpty().WithMessage("Maintenance end date is required.")
            .Must((req, end) => end >= req.MaintenanceStart)
            .WithMessage("Maintenance end date must be on or after the start date.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Cost)
            .GreaterThan(0).WithMessage("Cost must be greater than 0.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid maintenance status.");
    }
}
