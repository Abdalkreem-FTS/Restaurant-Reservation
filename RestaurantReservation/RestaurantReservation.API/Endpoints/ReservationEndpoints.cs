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
            .Accepts<CreateReservationRequest>("application/json")
            .WithValidation<CreateReservationRequest>()
            .WithSummary("Book a table. The restaurant and the table capacity follow from the table and are not accepted here.")
            .Produces<ReservationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:int}", Update)
            .WithName(nameof(Update))
            .Accepts<UpdateReservationRequest>("application/json")
            .WithValidation<UpdateReservationRequest>()
            .WithSummary("Replace a reservation.")
            .Produces<ReservationResponse>()
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
        IReservationRepository reservations,
        CancellationToken ct = default)
    {
        var result = await reservations.GetByIdAsync(id, ct);

        return result.Match(
            onValue: reservation => user.MayActFor(reservation.CustomerId)
                ? Results.Ok(reservation.ToResponse())
                : NotFound(id),
            onError: _ => NotFound(id));
    }

    private static async Task<IResult> ListByCustomer(
        int customerId,
        [AsParameters] PageParameters page,
        ClaimsPrincipal user,
        ICustomerRepository customers,
        IReservationRepository reservations,
        CancellationToken ct = default)
    {
        if (!user.MayActFor(customerId))
        {
            return NotYourCustomer("You can only see your own reservations.");
        }

        if ((await customers.GetByIdAsync(customerId, ct)).ProblemOrNull() is { } problem)
        {
            return problem;
        }

        var result = await reservations.GetReservationsByCustomerAsync(customerId, page.ToPageRequest(), ct);

        return Results.Ok(result.ToResponse());
    }

    private static async Task<IResult> ListOrders(
        int reservationId,
        [AsParameters] PageParameters page,
        ClaimsPrincipal user,
        IReservationRepository reservations,
        IOrderRepository orders,
        CancellationToken ct = default)
    {
        if (await Denied(reservationId, user, reservations, ct) is { } problem)
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
        IReservationRepository reservations,
        IOrderRepository orders,
        CancellationToken ct = default)
    {
        if (await Denied(reservationId, user, reservations, ct) is { } problem)
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
        if (!user.MayActFor(request.CustomerId))
        {
            return NotYourCustomer("You can only make reservations for yourself.");
        }

        var result = await service.CreateAsync(request, ct);

        return result.Match(
            onValue: reservation => Results.Created($"/api/reservations/{reservation.ReservationId}", reservation.ToResponse()),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> Update(
        int id,
        UpdateReservationRequest request,
        ClaimsPrincipal user,
        IReservationRepository reservations,
        IReservationService service,
        CancellationToken ct = default)
    {
        if (await Denied(id, user, reservations, ct) is { } problem)
        {
            return problem;
        }

        if (!user.MayActFor(request.CustomerId))
        {
            return NotYourCustomer("You can only make reservations for yourself.");
        }

        var result = await service.UpdateAsync(id, request, ct);

        return result.Match(
            onValue: reservation => Results.Ok(reservation.ToResponse()),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> Delete(
        int id,
        ClaimsPrincipal user,
        IReservationRepository reservations,
        IReservationService service,
        CancellationToken ct = default)
    {
        if (await Denied(id, user, reservations, ct) is { } problem)
        {
            return problem;
        }

        var result = await service.DeleteAsync(id, ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult?> Denied(
        int reservationId,
        ClaimsPrincipal user,
        IReservationRepository reservations,
        CancellationToken ct)
    {
        var result = await reservations.GetByIdAsync(reservationId, ct);

        return result.Match<IResult?>(
            onValue: reservation => user.MayActFor(reservation.CustomerId) ? null : NotFound(reservationId),
            onError: _ => NotFound(reservationId));
    }

    private static IResult NotFound(int id) =>
        Error.NotFound("Reservations.NotFound", $"Reservation with id {id} was not found.").ToProblem();

    private static IResult NotYourCustomer(string detail) =>
        Error.Forbidden("Reservations.NotYourCustomer", detail).ToProblem();
}
