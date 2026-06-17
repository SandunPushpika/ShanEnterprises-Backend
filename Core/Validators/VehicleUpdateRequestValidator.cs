using Core.DTOs.Request;
using FluentValidation;

namespace Core.Validators;

public class VehicleUpdateRequestValidator : AbstractValidator<VehicleUpdateRequest>
{
    public VehicleUpdateRequestValidator()
    {
        RuleFor(x => x.BrandId)
            .NotNull().WithMessage("Brand is required.")
            .GreaterThan(0).WithMessage("Invalid brand id.");

        RuleFor(x => x.TypeId)
            .NotNull().WithMessage("Vehicle type is required.")
            .GreaterThan(0).WithMessage("Invalid vehicle type id.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required.")
            .MaximumLength(50).WithMessage("Registration number cannot exceed 50 characters.");

        RuleFor(x => x.ManufactureYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
            .WithMessage($"Manufacture year must be between 1900 and {DateTime.UtcNow.Year + 1}.");

        RuleFor(x => x.Transmission)
            .NotNull().WithMessage("Transmission type is required.");

        RuleFor(x => x.Fuel)
            .NotNull().WithMessage("Fuel type is required.");

        RuleFor(x => x.SeatCapacity)
            .GreaterThan(0)
            .When(x => x.SeatCapacity.HasValue)
            .WithMessage("Seat capacity must be greater than 0.");

        RuleFor(x => x.LuggageCapacity)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LuggageCapacity.HasValue)
            .WithMessage("Luggage capacity cannot be negative.");

        RuleFor(x => x.DailyRentalPrice)
            .GreaterThan(0)
            .WithMessage("Daily rental price must be greater than 0.");

        RuleFor(x => x.PricePerKm)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PricePerKm.HasValue)
            .WithMessage("Price per kilometer cannot be negative.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description must be between 1000 characters.");

        RuleFor(x => x.MainImageUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.MainImageUrl))
            .WithMessage("Main image URL must be a valid URL.");
    }
}