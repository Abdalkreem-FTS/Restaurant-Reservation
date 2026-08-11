using FluentValidation;
using RestaurantReservation.API.Contracts.Tokens;

namespace RestaurantReservation.API.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty()
            .WithMessage("A username is required.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("A password is required.");
    }
}
