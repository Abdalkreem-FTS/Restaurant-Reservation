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
            })
            .AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
            {
                policy.RequireRole(Roles.Admin);
            });

        return services;
    }
}
