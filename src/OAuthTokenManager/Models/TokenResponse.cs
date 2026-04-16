namespace OAuthTokenManager.Models;

public record TokenResponse
{
    public required string AccessToken { get; init; }
    public string TokenType { get; init; } = "Bearer";
    public int ExpiresIn { get; init; }
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Returns true if the token has expired or will expire within 30 seconds.
    /// The 30-second buffer prevents using a token that expires mid-request.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= IssuedAt.AddSeconds(ExpiresIn - 30);
}
