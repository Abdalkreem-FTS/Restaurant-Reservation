using Riok.Mapperly.Abstractions;

namespace RestaurantReservation.API.Contracts.Orders;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class OrderMappers
{
    [MapProperty(nameof(Order.OrderItems), nameof(OrderResponse.Items))]
    private static partial OrderResponse ToResponse(this Order order);

    public static partial PagedResponse<OrderResponse> ToResponse(this PagedResult<Order> page);

    private static OrderItemResponse ToResponse(this OrderItem orderItem) =>
        new(
            orderItem.OrderItemId,
            orderItem.ItemId,
            orderItem.MenuItem?.Name,
            orderItem.Quantity,
            orderItem.UnitPrice);
}
