namespace OAuthTokenManager.Interfaces;

public interface ITokenManager
{
    /// <summary>
    /// Gets a valid access token, using the cache if available or requesting a new one if expired.
    /// </summary>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces the next call to GetAccessTokenAsync to request a fresh token.
    /// </summary>
    Task InvalidateTokenAsync();
}
