using OAuthTokenManager.Interfaces;
using OAuthTokenManager.Models;

namespace OAuthTokenManager.Services;

public class InMemoryTokenStore : ITokenStore
{
    private TokenResponse? _cachedToken;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<TokenResponse?> GetAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetAsync(TokenResponse token)
    {
        await _lock.WaitAsync();
        try
        {
            _cachedToken = token;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task ClearAsync()
    {
        await _lock.WaitAsync();
        try
        {
            _cachedToken = null;
        }
        finally
        {
            _lock.Release();
        }
    }
}
