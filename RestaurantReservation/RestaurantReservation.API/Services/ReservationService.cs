using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Services;

public class ReservationService(
    IReservationRepository reservationsRepository,
    ITableRepository tablesRepository,
    IUnitOfWork unitOfWork) : IReservationService
{
    public async Task<Result<Reservation>> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
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
        int reservationId,
        UpdateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await reservationsRepository.GetByIdAsync(reservationId, cancellationToken);

        if (existing.IsError)
        {
            return existing.Errors;
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

    public async Task<Result<Deleted>> DeleteAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        var deleted = await reservationsRepository.DeleteAsync(reservationId, cancellationToken);

        if (deleted.IsError)
        {
            return deleted.Errors;
        }

        var saved = await unitOfWork.SaveChangesAsync(cancellationToken);

        return saved.IsError ? saved.Errors : Result.Deleted;
    }

    private static void Apply(Reservation reservation, int customerId, int partySize, Table table)
    {
        reservation.CustomerId = customerId;
        reservation.PartySize = partySize;
        reservation.TableId = table.TableId;
        reservation.RestaurantId = table.RestaurantId;
        reservation.TableCapacity = table.Capacity;
    }

    private static Error TableNotFound(int tableId) => Error.Validation(
        "Reservations.TableNotFound",
        $"Table {tableId} does not exist.");
}
