using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace RestaurantReservation.API.Authentication;

public static class JwtBearerEventHandlers
{
    public const string LoggerCategory = "RestaurantReservation.API.Authentication.JwtBearer";

    private const string FailureDetailKey = "auth:failure-detail";

    public static JwtBearerEvents Create(ILogger logger) => new()
    {
        OnTokenValidated = async context =>
        {
            var tokenId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);

            if (string.IsNullOrEmpty(tokenId))
            {
                Reject(context, logger, "This token carries no 'jti', so it cannot be checked for revocation.");

                return;
            }

            var revoked = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocationStore>();

            if (await revoked.IsRevokedAsync(tokenId, context.HttpContext.RequestAborted))
            {
                Reject(context, logger, "This token has been revoked.");
            }
        },

        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("x-token-expired", "true");

                logger.LogInformation("Bearer token rejected: expired.");
            }
            else
            {
                logger.LogWarning(context.Exception, "Bearer token rejected.");
            }

            return Task.CompletedTask;
        },

        OnChallenge = async context =>
        {
            context.HandleResponse();

            await WriteProblem(
                context.HttpContext,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ChallengeDetail(context),
                "Auth.Unauthorized");
        },

        OnForbidden = context => WriteProblem(
            context.HttpContext,
            StatusCodes.Status403Forbidden,
            "Forbidden",
            "Your account does not have permission to use this endpoint.",
            "Auth.Forbidden"),
    };

    /// <summary>
    /// Written through the problem details service rather than serialized here, so a rejected token
    /// arrives in the same shape, with the same <c>traceId</c> and <c>errorCode</c>, as every other
    /// error this API reports.
    /// </summary>
    private static async Task WriteProblem(HttpContext context, int statusCode, string title, string detail, string errorCode)
    {
        context.Response.StatusCode = statusCode;

        var problemDetails = context.RequestServices.GetRequiredService<IProblemDetailsService>();

        await problemDetails.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Extensions = { ["errorCode"] = errorCode },
            }
        });
    }

    private static string ChallengeDetail(JwtBearerChallengeContext context)
    {
        if (context.HttpContext.Items[FailureDetailKey] is string detail)
        {
            return detail;
        }

        return string.IsNullOrEmpty(context.ErrorDescription) ? "A valid bearer token is required." : context.ErrorDescription;
    }

    /// <summary>
    /// Records the one reason a token was turned away: the caller reads it in the problem detail,
    /// the log carries it, and authentication fails with it.
    /// </summary>
    private static void Reject(TokenValidatedContext context, ILogger logger, string detail)
    {
        context.HttpContext.Items[FailureDetailKey] = detail;

        logger.LogInformation("Bearer token rejected: {Reason}", detail);

        context.Fail(detail);
    }
}
