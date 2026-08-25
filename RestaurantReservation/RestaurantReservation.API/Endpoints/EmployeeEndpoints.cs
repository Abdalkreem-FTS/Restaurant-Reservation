using System.Security.Claims;
using RestaurantReservation.API.Contracts.Employees;
using RestaurantReservation.Db.Models;

namespace RestaurantReservation.API.Endpoints;

public static class EmployeeEndpoints
{
    private const string Group = "Employees";

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
            .WithName($"{Group}.{nameof(ListManagers)}")
            .RequireAuthorization(AuthorizationPolicies.AdminOnly)
            .WithSummary("List every employee holding the Manager position. Administrators only.")
            .Produces<PagedResponse<EmployeeResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapGet("/{employeeId:int}/average-order-amount", GetAverageOrderAmount)
            .WithName($"{Group}.{nameof(GetAverageOrderAmount)}")
            .WithSummary("The average amount of one employee's orders, under the name the specification gives it.")
            .Produces<EmployeeAverageOrderAmountResponse>()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{employeeId:int}/statistics", GetStatistics)
            .WithName($"{Group}.{nameof(GetStatistics)}")
            .WithSummary("Order figures for one employee, the average among them. Managers and administrators may read anyone's, others only their own. An employee with no orders reports zeros.")
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

    private static async Task<IResult> GetAverageOrderAmount(
        int employeeId,
        ClaimsPrincipal user,
        IEmployeeRepository employees,
        CancellationToken ct = default)
    {
        var result = await ReadFigures(employeeId, user, employees, ct);

        return result.Match(
            onValue: statistics => Results.Ok(statistics.ToAverageOrderAmountResponse(employeeId)),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> GetStatistics(
        int employeeId,
        ClaimsPrincipal user,
        IEmployeeRepository employees,
        CancellationToken ct = default)
    {
        var result = await ReadFigures(employeeId, user, employees, ct);

        return result.Match(
            onValue: statistics => Results.Ok(statistics.ToResponse(employeeId)),
            onError: errors => errors.ToProblem());
    }

    /// <summary>
    /// The gate both figure endpoints share: may this caller read them, and does the employee exist.
    /// </summary>
    private static async Task<Result<OrderAmountStatistics>> ReadFigures(
        int employeeId,
        ClaimsPrincipal user,
        IEmployeeRepository employees,
        CancellationToken ct)
    {
        if (!user.MayReadFiguresFor(employeeId))
        {
            return Error.Forbidden(
                "Employees.FiguresNotYours",
                "Only a manager or an administrator can read another employee's order figures.");
        }

        var employee = await employees.GetByIdAsync(employeeId, ct);

        if (employee.IsError)
        {
            return employee.Errors;
        }

        return await employees.GetOrderAmountStatisticsAsync(employeeId, ct);
    }
}
