using System.Security.Claims;
using RestaurantReservation.API.Contracts.MenuItems;
using RestaurantReservation.API.Contracts.Orders;
using RestaurantReservation.API.Contracts.Reservations;

namespace RestaurantReservation.API.Endpoints;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reservations")
            .WithTags("Reservations")
            .RequireAuthorization(AuthorizationPolicies.Authenticated);

        MapGetEndpoints(group);
        MapMutationEndpoints(group);

        return app;
    }

    private static void MapGetEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("", List)
            .WithName(nameof(List))
            .RequireAuthorization(AuthorizationPolicies.StaffOnly)
            .WithSummary("List every reservation. Staff only.")
            .Produces<PagedResponse<ReservationResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapGet("/{id:int}", Get)
            .WithName(nameof(Get))
            .WithSummary("Read one reservation. A customer may read only their own; anyone else's is reported as missing.")
            .Produces<ReservationResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/customer/{customerId:int}", ListByCustomer)
            .WithName(nameof(ListByCustomer))
            .WithSummary("List one customer's reservations. A customer may ask only for their own.")
            .Produces<PagedResponse<ReservationResponse>>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{reservationId:int}/orders", ListOrders)
            .WithName(nameof(ListOrders))
            .WithSummary("List the orders placed on a reservation, each with its items. A reservation with none answers with an empty page, not 404.")
            .Produces<PagedResponse<OrderResponse>>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{reservationId:int}/menu-items", ListOrderedMenuItems)
            .WithName(nameof(ListOrderedMenuItems))
            .WithSummary("List the distinct menu items ordered within a reservation.")
            .Produces<PagedResponse<MenuItemResponse>>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static void MapMutationEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("", Create)
            .WithName(nameof(Create))
            .WithSummary("Book a table. The restaurant and the table capacity follow from the table and are not accepted here.")
            .Produces<ReservationResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", Update)
            .WithName(nameof(Update))
            .WithSummary("Replace a reservation.")
            .Produces<ReservationResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", Delete)
            .WithName(nameof(Delete))
            .WithSummary("Cancel a reservation.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> List(
        [AsParameters] PageParameters page,
        IReservationRepository reservations,
        CancellationToken ct = default)
    {
        var result = await reservations.GetAllAsync(page.ToPageRequest(), ct);

        return Results.Ok(result.ToResponse());
    }

    private static async Task<IResult> Get(
        int id,
        ClaimsPrincipal user,
        IReservationService service,
        CancellationToken ct = default)
    {
        var result = await service.GetAsync(user, id, ct);

        return result.Match(
            onValue: reservation => Results.Ok(reservation.ToResponse()),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> ListByCustomer(
        int customerId,
        [AsParameters] PageParameters page,
        ClaimsPrincipal user,
        IReservationService service,
        CancellationToken ct = default)
    {
        var result = await service.ListForCustomerAsync(user, customerId, page.ToPageRequest(), ct);

        return result.Match(
            onValue: reservations => Results.Ok(reservations.ToResponse()),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> ListOrders(
        int reservationId,
        [AsParameters] PageParameters page,
        ClaimsPrincipal user,
        IReservationService service,
        IOrderRepository orders,
        CancellationToken ct = default)
    {
        if ((await service.GetAsync(user, reservationId, ct)).ProblemOrNull() is { } problem)
        {
            return problem;
        }

        var result = await orders.ListOrdersAndMenuItemsAsync(reservationId, page.ToPageRequest(), ct);

        return Results.Ok(result.ToResponse());
    }

    private static async Task<IResult> ListOrderedMenuItems(
        int reservationId,
        [AsParameters] PageParameters page,
        ClaimsPrincipal user,
        IReservationService service,
        IOrderRepository orders,
        CancellationToken ct = default)
    {
        if ((await service.GetAsync(user, reservationId, ct)).ProblemOrNull() is { } problem)
        {
            return problem;
        }

        var result = await orders.ListOrderedMenuItemsAsync(reservationId, page.ToPageRequest(), ct);

        return Results.Ok(result.ToResponse());
    }

    private static async Task<IResult> Create(
        CreateReservationRequest request,
        ClaimsPrincipal user,
        IReservationService service,
        CancellationToken ct = default)
    {
        var result = await service.CreateAsync(user, request, ct);

        return result.Match(
            onValue: reservation => Results.Created($"/api/reservations/{reservation.ReservationId}", reservation.ToResponse()),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> Update(
        int id,
        UpdateReservationRequest request,
        ClaimsPrincipal user,
        IReservationService service,
        CancellationToken ct = default)
    {
        var result = await service.UpdateAsync(user, id, request, ct);

        return result.Match(
            onValue: reservation => Results.Ok(reservation.ToResponse()),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> Delete(
        int id,
        ClaimsPrincipal user,
        IReservationService service,
        CancellationToken ct = default)
    {
        var result = await service.DeleteAsync(user, id, ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}
