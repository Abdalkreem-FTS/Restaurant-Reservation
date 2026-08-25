using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace RestaurantReservation.API.Security;

public static class SecurityExtensions
{
    /// <summary>
    /// Everything the API needs to decide who a caller is and what they may do: how a token is
    /// issued, how one is validated and revoked, and which policies the endpoints name.
    /// </summary>
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<TokenRevocationOptions>()
            .Bind(configuration.GetSection(TokenRevocationOptions.SectionName));

        services.AddSingleton<JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        // The distributed cache exists for the revocation list, so it is configured alongside it.
        services.AddStackExchangeRedisCache(redis =>
        {
            redis.Configuration = configuration.GetConnectionString("Redis")
                                  ?? throw new InvalidOperationException("The 'Redis' connection string is required for token revocation.");

            redis.InstanceName = "restaurant-reservation:";
        });

        services.AddSingleton<ITokenRevocationStore, DistributedCacheTokenRevocationStore>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptionsMonitor<JwtOptions>, ILoggerFactory>((bearer, jwt, loggers) =>
            {
                bearer.MapInboundClaims = false;

                bearer.TokenValidationParameters = JwtTokenGenerator.CreateValidationParameters(jwt.CurrentValue);
                bearer.Events = JwtBearerEventHandlers.Create(loggers.CreateLogger(JwtBearerEventHandlers.LoggerCategory));
            });

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
