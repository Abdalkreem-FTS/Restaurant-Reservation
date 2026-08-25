using System.Security.Claims;
using FluentValidation;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Services;

/// <summary>
/// Owns who may touch a reservation and what a valid one looks like, so REST and gRPC decide
/// nothing themselves and only map the resulting <see cref="Error"/>.
/// </summary>
public class ReservationService(
    IReservationRepository reservationsRepository,
    ICustomerRepository customersRepository,
    ITableRepository tablesRepository,
    IValidator<CreateReservationRequest> createValidator,
    IValidator<UpdateReservationRequest> updateValidator,
    IUnitOfWork unitOfWork) : IReservationService
{
    public async Task<Result<Reservation>> GetAsync(
        ClaimsPrincipal caller,
        int reservationId,
        CancellationToken cancellationToken = default)
    {
        var existing = await reservationsRepository.GetByIdAsync(reservationId, cancellationToken);

        // Someone else's reservation is reported as missing rather than forbidden, so the response
        // does not confirm that it exists.
        if (existing.IsError || !caller.MayActFor(existing.Value.CustomerId))
        {
            return NotFound(reservationId);
        }

        return existing.Value;
    }

    public async Task<Result<PagedResult<Reservation>>> ListForCustomerAsync(
        ClaimsPrincipal caller,
        int customerId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        if (!caller.MayActFor(customerId))
        {
            return NotYourCustomer("You can only see your own reservations.");
        }

        var customer = await customersRepository.GetByIdAsync(customerId, cancellationToken);

        if (customer.IsError)
        {
            return customer.Errors;
        }

        return await reservationsRepository.GetReservationsByCustomerAsync(customerId, page, cancellationToken);
    }

    public async Task<Result<Reservation>> CreateAsync(
        ClaimsPrincipal caller,
        CreateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!caller.MayActFor(request.CustomerId))
        {
            return NotYourCustomer("You can only make reservations for yourself.");
        }

        var invalid = await Validate(createValidator, request, cancellationToken);

        if (invalid.Count > 0)
        {
            return invalid;
        }

        var table = await tablesRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (table.IsError)
        {
            return TableNotFound(request.TableId);
        }

        var reservation = new Reservation { ReservationDate = request.ReservationDate };

        Apply(reservation, request.CustomerId, request.PartySize, table.Value);

        var added = await reservationsRepository.AddAsync(reservation, cancellationToken);

        if (added.IsError)
        {
            return added.Errors;
        }

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);

        return saved.IsError ? saved.Errors : reservation;
    }

    public async Task<Result<Reservation>> UpdateAsync(
        ClaimsPrincipal caller,
        int reservationId,
        UpdateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(caller, reservationId, cancellationToken);

        if (existing.IsError)
        {
            return existing.Errors;
        }

        if (!caller.MayActFor(request.CustomerId))
        {
            return NotYourCustomer("You can only make reservations for yourself.");
        }

        var invalid = await Validate(updateValidator, request, cancellationToken);

        if (invalid.Count > 0)
        {
            return invalid;
        }

        var table = await tablesRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (table.IsError)
        {
            return TableNotFound(request.TableId);
        }

        var reservation = existing.Value;

        reservation.ReservationDate = request.ReservationDate;

        Apply(reservation, request.CustomerId, request.PartySize, table.Value);

        var updated = reservationsRepository.Update(reservation);

        if (updated.IsError)
        {
            return updated.Errors;
        }

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);

        return saved.IsError ? saved.Errors : reservation;
    }

    public async Task<Result<Deleted>> DeleteAsync(
        ClaimsPrincipal caller,
        int reservationId,
        CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(caller, reservationId, cancellationToken);

        if (existing.IsError)
        {
            return existing.Errors;
        }

        var deleted = await reservationsRepository.DeleteAsync(reservationId, cancellationToken);

        if (deleted.IsError)
        {
            return deleted.Errors;
        }

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);

        return saved.IsError ? saved.Errors : Result.Deleted;
    }

    private static async Task<List<Error>> Validate<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        return result.IsValid
            ? []
            : result.Errors.Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage)).ToList();
    }

    private static void Apply(Reservation reservation, int customerId, int partySize, Table table)
    {
        reservation.CustomerId = customerId;
        reservation.PartySize = partySize;
        reservation.TableId = table.TableId;
        reservation.RestaurantId = table.RestaurantId;
        reservation.TableCapacity = table.Capacity;
    }

    private static Error NotFound(int reservationId) => Error.NotFound(
        "Reservations.NotFound",
        $"Reservation with id {reservationId} was not found.");

    private static Error NotYourCustomer(string description) => Error.Forbidden(
        "Reservations.NotYourCustomer",
        description);

    private static Error TableNotFound(int tableId) => Error.Failure(
        "Reservations.TableNotFound",
        $"Table {tableId} does not exist.");
}
