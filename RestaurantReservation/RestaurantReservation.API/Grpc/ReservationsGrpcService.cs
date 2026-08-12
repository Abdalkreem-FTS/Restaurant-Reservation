using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantReservation.API.Grpc;

[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public class ReservationsGrpcService(
    IReservationService reservationService,
    IReservationRepository reservationRepository)
    : Reservations.ReservationsBase
{
    public override async Task<ReservationMessage> Create(CreateReservationCommand request, ServerCallContext context)
    {
        var result = await reservationService.CreateAsync(context.GetHttpContext().User, request.ToRequest(), context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<ReservationMessage> Get(GetReservationQuery request, ServerCallContext context)
    {
        var result = await reservationService.GetAsync(context.GetHttpContext().User, request.ReservationId, context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<ReservationMessage> Update(UpdateReservationCommand request, ServerCallContext context)
    {
        var result = await reservationService.UpdateAsync(
            context.GetHttpContext().User,
            request.ReservationId,
            request.ToRequest(),
            context.CancellationToken);

        return result.Match(
            onValue: reservation => reservation.ToMessage(),
            onError: errors => throw errors.ToRpcException());
    }

    public override async Task<Empty> Delete(DeleteReservationCommand request, ServerCallContext context)
    {
        var result = await reservationService.DeleteAsync(context.GetHttpContext().User, request.ReservationId, context.CancellationToken);

        return result.Match(
            onValue: _ => new Empty(),
            onError: errors => throw errors.ToRpcException());
    }

    [Authorize(Policy = AuthorizationPolicies.StaffOnly)]
    public override async Task<ReservationPage> List(ListReservationsQuery request, ServerCallContext context)
    {
        var page = await reservationRepository.GetAllAsync(request.ToPageRequest(), context.CancellationToken);

        return page.ToMessage();
    }
}
