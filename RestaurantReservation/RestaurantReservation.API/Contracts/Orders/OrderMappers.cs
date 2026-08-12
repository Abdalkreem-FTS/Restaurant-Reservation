namespace RestaurantReservation.API.Contracts.Orders;

public static class OrderMappers
{
    public static OrderResponse ToResponse(this Order order) =>
        new(
            order.OrderId,
            order.ReservationId,
            order.EmployeeId,
            order.OrderDate,
            order.TotalAmount,
            [.. order.OrderItems.Select(ToResponse)]);

    public static PagedResponse<OrderResponse> ToResponse(this PagedResult<Order> page) =>
        page.ToPagedResponse(ToResponse);

    private static OrderItemResponse ToResponse(this OrderItem orderItem) =>
        new(
            orderItem.OrderItemId,
            orderItem.ItemId,
            orderItem.MenuItem?.Name,
            orderItem.Quantity,
            orderItem.UnitPrice);
}
