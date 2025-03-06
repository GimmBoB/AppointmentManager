namespace AppointmentManager.Shared;

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessExpirationDateTimeUtc,
    DateTime RefreshExpirationDateTimeUtc,
    int AccessExpiresInSeconds,
    int RefreshExpiresInSeconds)
{
    public static TokenDto Empty =>
        new TokenDto(string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue, 0, 0);
}