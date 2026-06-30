using Core.DTOs.Request.Auth;
using FluentValidation;

namespace Core.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(request => request.Password).NotEmpty().WithMessage("Password is required");
    }
}