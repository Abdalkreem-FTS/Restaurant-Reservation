using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Grpc;

[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public class ReservationsGrpcService(
    IReservationService service,
    IReservationRepository reservations,
    IValidator<CreateReservationRequest> createValidator,
    IValidator<UpdateReservationRequest> updateValidator)
    : Reservations.ReservationsBase
{
    public override async Task<ReservationMessage> Create(CreateReservationCommand command, ServerCallContext context)
    {
        var request = command.ToRequest();

        await Validate(createValidator, request, context);

        if (!context.GetHttpContext().User.MayActFor(request.CustomerId))
        {
            throw NotYourCustomer();
        }

        var result = await service.CreateAsync(request, context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<ReservationMessage> Get(GetReservationQuery query, ServerCallContext context)
    {
        var result = await reservations.GetByIdAsync(query.ReservationId, context.CancellationToken);
        var user = context.GetHttpContext().User;

        return result.Match(
            onValue: reservation => user.MayActFor(reservation.CustomerId)
                ? reservation.ToMessage()
                : throw NotFound(query.ReservationId),
            onError: _ => throw NotFound(query.ReservationId));
    }

    public override async Task<ReservationMessage> Update(UpdateReservationCommand command, ServerCallContext context)
    {
        var request = command.ToRequest();

        await Validate(updateValidator, request, context);

        await EnsureMine(command.ReservationId, context);

        if (!context.GetHttpContext().User.MayActFor(request.CustomerId))
        {
            throw NotYourCustomer();
        }

        var result = await service.UpdateAsync(command.ReservationId, request, context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<Empty> Delete(DeleteReservationCommand command, ServerCallContext context)
    {
        await EnsureMine(command.ReservationId, context);

        var result = await service.DeleteAsync(command.ReservationId, context.CancellationToken);

        return result.Match(
            onValue: _ => new Empty(),
            onError: errors => throw errors.ToRpcException());
    }

    [Authorize(Policy = AuthorizationPolicies.StaffOnly)]
    public override async Task<ReservationPage> List(ListReservationsQuery query, ServerCallContext context)
    {
        var page = await reservations.GetAllAsync(query.ToPageRequest(), context.CancellationToken);

        return page.ToMessage();
    }

    private static async Task Validate<T>(IValidator<T> validator, T request, ServerCallContext context)
    {
        var result = await validator.ValidateAsync(request, context.CancellationToken);

        if (result.IsValid)
        {
            return;
        }

        throw result.Errors
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .ToList()
            .ToRpcException();
    }

    private async Task EnsureMine(int reservationId, ServerCallContext context)
    {
        var result = await reservations.GetByIdAsync(reservationId, context.CancellationToken);
        var user = context.GetHttpContext().User;

        var denied = result.Match(
            onValue: reservation => user.MayActFor(reservation.CustomerId) ? null : NotFound(reservationId),
            onError: _ => NotFound(reservationId));

        if (denied is not null)
        {
            throw denied;
        }
    }

    private static RpcException NotFound(int reservationId) =>
        Error.NotFound("Reservations.NotFound", $"Reservation with id {reservationId} was not found.").ToRpcException();

    private static RpcException NotYourCustomer() =>
        Error.Forbidden("Reservations.NotYourCustomer", "You can only make reservations for yourself.").ToRpcException();
}
