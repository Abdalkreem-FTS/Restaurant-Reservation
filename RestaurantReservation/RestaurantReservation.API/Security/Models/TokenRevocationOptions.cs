namespace RestaurantReservation.API.Security.Models;

public sealed class TokenRevocationOptions
{
    public const string SectionName = "TokenRevocation";

    public bool FailOpenOnCacheFailure { get; init; }
}
