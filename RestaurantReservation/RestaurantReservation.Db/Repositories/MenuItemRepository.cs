using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class MenuItemRepository(RestaurantReservationDbContext context, ILogger<MenuItemRepository> logger) : Repository<MenuItem>(context, logger), IMenuItemRepository;
