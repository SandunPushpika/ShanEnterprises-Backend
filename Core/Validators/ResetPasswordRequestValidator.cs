using Core.DTOs.Request.Auth;
using FluentValidation;

namespace Core.Validators;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is Invalid");

        RuleFor(request => request.Code)
            .NotEmpty()
            .WithMessage("Verification code is required");

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
