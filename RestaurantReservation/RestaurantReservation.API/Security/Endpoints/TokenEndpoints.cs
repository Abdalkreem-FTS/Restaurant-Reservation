using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

namespace RestaurantReservation.API.Security.Endpoints;

public static class TokenEndpoints
{
    private const string Group = "Tokens";

    public static IEndpointRouteBuilder MapTokenEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tokens").WithTags("Tokens");

        group.MapPost("", SignIn)
            .WithName($"{Group}.{nameof(SignIn)}")
            .WithValidation<LoginRequest>()
            .WithSummary("Sign in and receive a bearer token. Every seeded user has the password Password123!.")
            .Produces<TokenResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapDelete("", SignOut)
            .WithName($"{Group}.{nameof(SignOut)}")
            .RequireAuthorization(AuthorizationPolicies.Authenticated)
            .WithSummary("Sign out, revoking the token used to make this call until it would have expired anyway.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        return app;
    }

    private static async Task<IResult> SignIn(
        LoginRequest request,
        IUserRepository users,
        IPasswordHasher<User> passwordHasher,
        JwtTokenGenerator tokens,
        CancellationToken ct = default)
    {
        var result = await users.FindByUsernameAsync(request.Username, ct);

        return result.Match(
            onValue: user => passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed
                ? InvalidCredentials()
                : Results.Ok(tokens.GenerateToken(user)),
            onError: _ => InvalidCredentials());
    }

    private static async Task<IResult> SignOut(
        ClaimsPrincipal user,
        ITokenRevocationStore revoked,
        CancellationToken ct = default)
    {
        var tokenId = user.FindFirstValue(JwtRegisteredClaimNames.Jti)!;
        var expiresAt = long.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Exp)!, CultureInfo.InvariantCulture);

        var wasRevoked = await revoked.RevokeAsync(tokenId, DateTimeOffset.FromUnixTimeSeconds(expiresAt), ct);

        return wasRevoked
            ? Results.NoContent()
            : Error.Unavailable("Auth.RevocationUnavailable", "Your sign-out could not be recorded. Try again shortly.").ToProblem();
    }

    private static IResult InvalidCredentials() =>
        Error.Unauthorized("Auth.InvalidCredentials", "The username or password is incorrect.").ToProblem();
}
