namespace RestaurantReservation.Db.Entities;

/// <summary>
/// Marks an entity as carrying an optimistic concurrency token. Implementing this is what enrolls a type in lost-update detection.
/// </summary>
public interface IVersionedEntity
{
    byte[] RowVersion { get; set; }
}
