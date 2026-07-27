using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class OrderItemRepository(RestaurantReservationDbContext context) : Repository<OrderItem>(context), IOrderItemRepository;
