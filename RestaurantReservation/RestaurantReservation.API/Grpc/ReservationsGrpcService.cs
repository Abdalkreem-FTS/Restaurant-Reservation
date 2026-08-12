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
    public override async Task<ReservationMessage> Create(CreateReservationCommand request, ServerCallContext context)
    {
        if (!context.GetHttpContext().User.MayActFor(request.CustomerId))
        {
            throw NotYourCustomer();
        }

        var createRequest = request.ToRequest();

        await Validate(createValidator, createRequest, context);

        var result = await service.CreateAsync(createRequest, context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<ReservationMessage> Get(GetReservationQuery request, ServerCallContext context)
    {
        var result = await reservations.GetByIdAsync(request.ReservationId, context.CancellationToken);
        var user = context.GetHttpContext().User;

        return result.Match(
            onValue: reservation => user.MayActFor(reservation.CustomerId)
                ? reservation.ToMessage()
                : throw NotFound(request.ReservationId),
            onError: _ => throw NotFound(request.ReservationId));
    }

    public override async Task<ReservationMessage> Update(UpdateReservationCommand request, ServerCallContext context)
    {
        await EnsureMine(request.ReservationId, context);

        if (!context.GetHttpContext().User.MayActFor(request.CustomerId))
        {
            throw NotYourCustomer();
        }

        var updateRequest = request.ToRequest();

        await Validate(updateValidator, updateRequest, context);

        var result = await service.UpdateAsync(request.ReservationId, updateRequest, context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<Empty> Delete(DeleteReservationCommand request, ServerCallContext context)
    {
        await EnsureMine(request.ReservationId, context);

        var result = await service.DeleteAsync(request.ReservationId, context.CancellationToken);

        return result.Match(
            onValue: _ => new Empty(),
            onError: errors => throw errors.ToRpcException());
    }

    [Authorize(Policy = AuthorizationPolicies.StaffOnly)]
    public override async Task<ReservationPage> List(ListReservationsQuery request, ServerCallContext context)
    {
        var page = await reservations.GetAllAsync(request.ToPageRequest(), context.CancellationToken);

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
