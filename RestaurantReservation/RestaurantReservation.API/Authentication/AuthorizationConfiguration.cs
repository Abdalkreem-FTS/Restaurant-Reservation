namespace RestaurantReservation.API.Authentication;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.Authenticated, policy =>
            {
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy(AuthorizationPolicies.StaffOnly, policy =>
            {
                policy.RequireRole(Roles.Employee);
            });

        return services;
    }
}
