namespace RestaurantReservation.Db.Models;

public sealed record OrderAmountStatistics(int Count, decimal Sum, decimal Average, decimal Min, decimal Max, decimal Variance);
