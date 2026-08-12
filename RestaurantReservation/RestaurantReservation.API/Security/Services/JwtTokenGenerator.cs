using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace RestaurantReservation.API.Security.Services;

public sealed class JwtTokenGenerator(IOptionsMonitor<JwtOptions> options)
{
    private const string TokenType = "Bearer";
    private const string RoleClaimType = "role";

    public const string CustomerIdClaimType = "customerId";

    public const string EmployeeIdClaimType = "employeeId";

    private static readonly JsonWebTokenHandler Handler = new();

    public static TokenValidationParameters CreateValidationParameters(JwtOptions options) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = options.Issuer,

        ValidateAudience = true,
        ValidAudience = options.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SigningKey(options),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        NameClaimType = JwtRegisteredClaimNames.Name,
        RoleClaimType = RoleClaimType
    };

    public TokenResponse GenerateToken(User user)
    {
        var jwt = options.CurrentValue;

        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(jwt.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, user.Username)
        };

        claims.AddRange(Roles.For(user).Select(role => new Claim(RoleClaimType, role)));

        if (user.Customer is { } customer)
        {
            claims.Add(new Claim(CustomerIdClaimType, customer.CustomerId.ToString()));
        }

        if (user.Employee is { } employee)
        {
            claims.Add(new Claim(EmployeeIdClaimType, employee.EmployeeId.ToString()));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            Subject = new ClaimsIdentity(claims),

            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,

            SigningCredentials = new SigningCredentials(SigningKey(jwt), SecurityAlgorithms.HmacSha256),
        };

        return new TokenResponse(Handler.CreateToken(descriptor), TokenType, expiresAt);
    }

    private static SymmetricSecurityKey SigningKey(JwtOptions options) => new(Encoding.UTF8.GetBytes(options.SecurityKey));
}
