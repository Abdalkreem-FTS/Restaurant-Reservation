using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;
using RestaurantReservation.Db.Enums;

namespace RestaurantReservation.Db.Repositories;

public class EmployeeRepository(RestaurantReservationDbContext context) : Repository<Employee>(context), IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> ListManagersAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Employees
            .Where(e => e.Position == EmployeePosition.Manager)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> CalculateAverageOrderAmountAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var orders = Context.Orders.Where(o => o.EmployeeId == employeeId);

        return await orders.AnyAsync(cancellationToken) ? await orders.AverageAsync(o => o.TotalAmount, cancellationToken) : 0m;
    }

    public async Task<IReadOnlyList<EmployeeRestaurantDetail>> GetEmployeesWithRestaurantAsync(CancellationToken cancellationToken = default)
    {
        return await Context.EmployeeDetails.ToListAsync(cancellationToken);
    }
}