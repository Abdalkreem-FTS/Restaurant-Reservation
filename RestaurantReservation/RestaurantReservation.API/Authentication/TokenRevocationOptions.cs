namespace RestaurantReservation.API.Authentication;

public sealed class TokenRevocationOptions
{
    public const string SectionName = "TokenRevocation";

    public bool FailOpenOnCacheFailure { get; init; }
}
