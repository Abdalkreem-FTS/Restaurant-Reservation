namespace RestaurantReservation.API.Contracts.Tokens;

public sealed record TokenResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
