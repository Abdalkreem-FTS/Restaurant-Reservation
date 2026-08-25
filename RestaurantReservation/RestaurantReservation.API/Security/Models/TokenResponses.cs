namespace RestaurantReservation.API.Security.Models;

public sealed record TokenResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
