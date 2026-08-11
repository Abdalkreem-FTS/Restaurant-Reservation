using System.Security.Claims;
using RestaurantReservation.API.Contracts.Employees;

namespace RestaurantReservation.API.Endpoints;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees")
            .RequireAuthorization(AuthorizationPolicies.StaffOnly);

        MapGetEndpoints(group);

        return app;
    }

    private static void MapGetEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/managers", ListManagers)
            .WithName(nameof(ListManagers))
            .WithSummary("List every employee holding the Manager position.")
            .Produces<PagedResponse<EmployeeResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapGet("/{employeeId:int}/statistics", GetStatistics)
            .WithName(nameof(GetStatistics))
            .WithSummary("Order figures for one employee. Managers may read anyone's, others only their own. An employee with no orders reports zeros.")
            .Produces<EmployeeStatisticsResponse>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ListManagers(
        [AsParameters] PageParameters page,
        IEmployeeRepository employees,
        CancellationToken ct = default)
    {
        var result = await employees.ListManagersAsync(page.ToPageRequest(), ct);

        return Results.Ok(result.ToResponse());
    }

    private static async Task<IResult> GetStatistics(
        int employeeId,
        ClaimsPrincipal user,
        IEmployeeRepository employees,
        CancellationToken ct = default)
    {
        if (!user.MayReadFiguresFor(employeeId))
        {
            return Error.Forbidden(
                "Employees.FiguresNotYours",
                "Only a manager can read another employee's order figures.").ToProblem();
        }

        if ((await employees.GetByIdAsync(employeeId, ct)).ProblemOrNull() is { } problem)
        {
            return problem;
        }

        var statistics = await employees.GetOrderAmountStatisticsAsync(employeeId, ct);

        return Results.Ok(statistics.ToResponse(employeeId));
    }
}
