using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class OrderItemRepository(RestaurantReservationDbContext context, ILogger<OrderItemRepository> logger) : Repository<OrderItem>(context, logger), IOrderItemRepository;
