using RestaurantReservation.Db.Models;

namespace RestaurantReservation.API.Contracts.Employees;

public static class EmployeeMappers
{
    public static EmployeeResponse ToResponse(this Employee employee) =>
        new(
            employee.EmployeeId,
            employee.RestaurantId,
            employee.FirstName,
            employee.LastName,
            employee.Position.ToString());

    public static PagedResponse<EmployeeResponse> ToResponse(this PagedResult<Employee> page) =>
        page.ToPagedResponse(ToResponse);

    public static EmployeeStatisticsResponse ToResponse(this OrderAmountStatistics statistics, int employeeId) =>
        new(
            employeeId,
            statistics.Count,
            statistics.Sum,
            statistics.Average,
            statistics.Min,
            statistics.Max,
            statistics.Variance);
}
