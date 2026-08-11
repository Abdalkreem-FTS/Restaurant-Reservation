using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;
using RestaurantReservation.Db.Enums;
using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Repositories;

public class EmployeeRepository(RestaurantReservationDbContext context, ILogger<EmployeeRepository> logger) : Repository<Employee>(context, logger), IEmployeeRepository
{
    public async Task<PagedResult<Employee>> ListManagersAsync(PageRequest page, CancellationToken cancellationToken = default)
    {
        return await Context.Employees
            .Where(e => e.Position == EmployeePosition.Manager)
            .AsNoTracking()
            .OrderBy(e => e.EmployeeId)
            .GetPageAsync(page, cancellationToken);
    }

    /// <summary>
    /// Aggregates in one query and finishes the arithmetic in memory. <c>Variance</c> is the
    /// population variance — divided by the number of orders, not by one fewer — computed as <c>E[X²] − (E[X])²</c>.
    /// </summary>
    public async Task<OrderAmountStatistics> GetOrderAmountStatisticsAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var aggregates = await Context.Orders
            .Where(o => o.EmployeeId == employeeId)
            .GroupBy(o => o.EmployeeId)
            .Select(g => new
            {
                Count = g.Count(),
                Sum = g.Sum(o => o.TotalAmount),
                Min = g.Min(o => o.TotalAmount),
                Max = g.Max(o => o.TotalAmount),
                SumOfSquares = g.Sum(o => o.TotalAmount * o.TotalAmount),
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (aggregates is null)
        {
            return new OrderAmountStatistics(0, 0m, 0m, 0m, 0m, 0m);
        }

        var mean = aggregates.Sum / aggregates.Count;
        var variance = Math.Max(0m, aggregates.SumOfSquares / aggregates.Count - mean * mean);

        return new OrderAmountStatistics(aggregates.Count, aggregates.Sum, mean, aggregates.Min, aggregates.Max, variance);
    }

    public async Task<PagedResult<EmployeeRestaurantDetail>> GetEmployeesWithRestaurantAsync(PageRequest page, CancellationToken cancellationToken = default)
    {
        return await Context.EmployeeDetails
            .OrderBy(e => e.EmployeeId)
            .GetPageAsync(page, cancellationToken);
    }
}
