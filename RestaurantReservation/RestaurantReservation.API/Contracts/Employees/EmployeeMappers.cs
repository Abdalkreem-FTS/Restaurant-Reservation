using RestaurantReservation.Db.Models;
using Riok.Mapperly.Abstractions;

namespace RestaurantReservation.API.Contracts.Employees;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EmployeeMappers
{
    private static partial EmployeeResponse ToResponse(this Employee employee);

    public static partial PagedResponse<EmployeeResponse> ToResponse(this PagedResult<Employee> page);

    extension(OrderAmountStatistics statistics)
    {
        public EmployeeAverageOrderAmountResponse ToAverageOrderAmountResponse(int employeeId) =>
            new(employeeId, statistics.Average);

        public EmployeeStatisticsResponse ToResponse(int employeeId) =>
            new(
                employeeId,
                statistics.Count,
                statistics.Sum,
                statistics.Average,
                statistics.Min,
                statistics.Max,
                statistics.Variance);
    }
}
