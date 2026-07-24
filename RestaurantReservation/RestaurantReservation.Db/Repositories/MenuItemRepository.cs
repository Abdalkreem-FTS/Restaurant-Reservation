using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class MenuItemRepository(RestaurantReservationDbContext context) : Repository<MenuItem>(context), IMenuItemRepository;