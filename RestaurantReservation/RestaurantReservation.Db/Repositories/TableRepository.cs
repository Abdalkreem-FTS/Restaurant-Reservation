using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class TableRepository(RestaurantReservationDbContext context) : Repository<Table>(context), ITableRepository;