using Core.DTOs.Request.Review;
using FluentValidation;

namespace Core.Validators;

public class ReviewRequestValidator: AbstractValidator<AddReviewRequest>
{
    public ReviewRequestValidator()
    {
        RuleFor(x => x.VehicleRating)
            .NotEmpty()
            .WithMessage("Vehicle rating is required")
            .InclusiveBetween(1, 5)
            .WithMessage("Vehicle rating must be between 1 and 5");


        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .WithMessage("Comment cannot exceed 500 characters");
    }
}