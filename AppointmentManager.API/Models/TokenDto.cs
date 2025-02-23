namespace AppointmentManager.API.Models;

public record TokenDto(string AccessToken, string RefreshToken, DateTime AccessExpirationDateTimeUtc, DateTime RefreshExpirationDateTimeUtc, int AccessExpiresInSeconds, int RefreshExpiresInSeconds);