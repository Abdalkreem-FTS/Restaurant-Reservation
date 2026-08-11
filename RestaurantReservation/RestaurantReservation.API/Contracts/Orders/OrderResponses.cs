namespace RestaurantReservation.API.Contracts.Orders;

public sealed record OrderResponse(
    int OrderId,
    int ReservationId,
    int EmployeeId,
    DateTime OrderDate,
    decimal TotalAmount,
    IReadOnlyList<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    int OrderItemId,
    int ItemId,
    string? MenuItemName,
    int Quantity,
    decimal UnitPrice);
