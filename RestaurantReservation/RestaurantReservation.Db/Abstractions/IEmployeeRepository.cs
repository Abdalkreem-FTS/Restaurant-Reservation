using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Entities.Views;

namespace RestaurantReservation.Db.Abstractions;

public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>Retrieves all employees holding the Manager position.</summary>
    Task<IReadOnlyList<Employee>> ListManagersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the average total amount across all orders handled by the given employee.
    /// Returns 0 when the employee has no orders.
    /// </summary>
    Task<decimal> CalculateAverageOrderAmountAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every employee together with their restaurant details from the database view.</summary>
    Task<IReadOnlyList<EmployeeRestaurantDetail>> GetEmployeesWithRestaurantAsync(CancellationToken cancellationToken = default);
}