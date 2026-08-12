namespace RestaurantReservation.API.Authentication;

public static class AuthorizationPolicies
{
    public const string Authenticated = nameof(Authenticated);

    public const string StaffOnly = nameof(StaffOnly);

    public const string AdminOnly = nameof(AdminOnly);
}
