using OAuthTokenManager.Interfaces;
using OAuthTokenManager.Models;
using OAuthTokenManager.Services;

var builder = WebApplication.CreateBuilder(args);

var tokenConfig = new TokenRequest(
    TokenEndpoint: builder.Configuration["OAuth:TokenEndpoint"] ?? "https://example.com/oauth2/token",
    ClientId: builder.Configuration["OAuth:ClientId"] ?? "replace-client-id",
    ClientSecret: builder.Configuration["OAuth:ClientSecret"] ?? "replace-client-secret",
    Scope: builder.Configuration["OAuth:Scope"] ?? "payments.read payments.write");

builder.Services.AddSingleton(tokenConfig);
builder.Services.AddSingleton<ITokenStore, InMemoryTokenStore>();
builder.Services.AddHttpClient<ITokenManager, TokenManager>();

var app = builder.Build();

app.MapGet("/token", async (ITokenManager tokenManager, CancellationToken cancellationToken) =>
{
    var token = await tokenManager.GetAccessTokenAsync(cancellationToken);
    return Results.Ok(new { accessToken = token });
});

app.MapPost("/token/invalidate", async (ITokenManager tokenManager) =>
{
    await tokenManager.InvalidateTokenAsync();
    return Results.NoContent();
});

app.Run();
