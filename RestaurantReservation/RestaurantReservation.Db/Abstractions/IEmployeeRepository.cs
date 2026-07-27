using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;
using RestaurantReservation.Db.Models;
using RestaurantReservation.Db.Pagination;

namespace RestaurantReservation.Db.Abstractions;

public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>Retrieves a page of employees holding the Manager position, ordered by id.</summary>
    Task<PagedResult<Employee>> ListManagersAsync(PageRequest page, CancellationToken cancellationToken = default);

    /// <summary>
    /// Computes count, sum, average, min, max and variance across all orders handled by the given
    /// employee, in a single query. All values are 0 when the employee has no orders.
    /// </summary>
    Task<OrderAmountStatistics> GetOrderAmountStatisticsAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a page of employees with their restaurant details from the database view.</summary>
    Task<PagedResult<EmployeeRestaurantDetail>> GetEmployeesWithRestaurantAsync(PageRequest page, CancellationToken cancellationToken = default);
}
