using System.Net.Http.Json;
using OAuthTokenManager.Interfaces;
using OAuthTokenManager.Models;

namespace OAuthTokenManager.Services;

public class TokenManager : ITokenManager
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;
    private readonly TokenRequest _tokenRequest;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public TokenManager(HttpClient httpClient, ITokenStore tokenStore, TokenRequest tokenRequest)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
        _tokenRequest = tokenRequest;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _tokenStore.GetAsync();
        if (cached is not null && !cached.IsExpired)
        {
            return cached.AccessToken;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            cached = await _tokenStore.GetAsync();
            if (cached is not null && !cached.IsExpired)
            {
                return cached.AccessToken;
            }

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _tokenRequest.ClientId,
                ["client_secret"] = _tokenRequest.ClientSecret
            };

            if (!string.IsNullOrWhiteSpace(_tokenRequest.Scope))
            {
                formData["scope"] = _tokenRequest.Scope;
            }

            using var content = new FormUrlEncodedContent(formData);
            using var response = await _httpClient.PostAsync(_tokenRequest.TokenEndpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<OAuthWireResponse>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("OAuth token response was empty.");

            var token = new TokenResponse
            {
                AccessToken = payload.access_token,
                TokenType = payload.token_type ?? "Bearer",
                ExpiresIn = payload.expires_in,
                IssuedAt = DateTime.UtcNow
            };

            await _tokenStore.SetAsync(token);
            return token.AccessToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public Task InvalidateTokenAsync() => _tokenStore.ClearAsync();

    private sealed record OAuthWireResponse(
        string access_token,
        string? token_type,
        int expires_in);
}
