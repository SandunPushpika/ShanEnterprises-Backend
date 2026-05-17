using Core.DTOs.Request.Auth;
using FluentValidation;

namespace Core.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is Invalid");
        
        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Password is Invalid");
    }    
}