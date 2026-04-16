namespace OAuthTokenManager.Models;

/// <summary>
/// Configuration for an OAuth 2.0 client credentials token request.
/// </summary>
public record TokenRequest(
    string TokenEndpoint,
    string ClientId,
    string ClientSecret,
    string Scope = "");
