using Microsoft.Extensions.Logging;
using RestaurantReservation.Db.Abstractions;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public class TableRepository(RestaurantReservationDbContext context, ILogger<TableRepository> logger) : Repository<Table>(context, logger), ITableRepository;
