using OAuthTokenManager.Models;

namespace OAuthTokenManager.Interfaces;

/// <summary>
/// Abstraction for token persistence. Swap InMemoryTokenStore for a Redis or DB-backed
/// implementation without changing TokenManager.
/// </summary>
public interface ITokenStore
{
    Task<TokenResponse?> GetAsync();
    Task SetAsync(TokenResponse token);
    Task ClearAsync();
}
