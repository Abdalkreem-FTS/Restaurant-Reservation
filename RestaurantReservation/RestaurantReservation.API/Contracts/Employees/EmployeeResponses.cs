namespace RestaurantReservation.API.Contracts.Employees;

public sealed record EmployeeResponse(
    int EmployeeId,
    int RestaurantId,
    string FirstName,
    string LastName,
    string Position);

public sealed record EmployeeAverageOrderAmountResponse(
    int EmployeeId,
    decimal AverageOrderAmount);

public sealed record EmployeeStatisticsResponse(
    int EmployeeId,
    int OrderCount,
    decimal TotalAmount,
    decimal AverageAmount,
    decimal MinimumAmount,
    decimal MaximumAmount,
    decimal Variance);
