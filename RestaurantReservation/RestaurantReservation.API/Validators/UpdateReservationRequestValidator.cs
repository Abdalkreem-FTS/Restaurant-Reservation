using FluentValidation;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Validators;

public class UpdateReservationRequestValidator : AbstractValidator<UpdateReservationRequest>
{
    public UpdateReservationRequestValidator(TimeProvider time)
    {
        RuleFor(request => request.CustomerId)
            .GreaterThan(0)
            .WithMessage("A customer must be selected.");

        RuleFor(request => request.TableId)
            .GreaterThan(0)
            .WithMessage("A table must be selected.");

        RuleFor(request => request.PartySize)
            .GreaterThan(0)
            .WithMessage("Party size must be at least one guest.");

        RuleFor(request => request.ReservationDate)
            .Must(date => date.Ticks % TimeSpan.TicksPerHour == 0)
            .WithMessage("Reservations can only start exactly on the hour.");

        RuleFor(request => request.ReservationDate)
            .GreaterThan(_ => time.GetUtcNow())
            .WithMessage("A reservation cannot be moved to a time that has already passed.");
    }
}
