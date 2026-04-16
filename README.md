# OAuthTokenManager

A reusable .NET 8 library that implements OAuth 2.0 client-credentials token retrieval, in-memory caching, invalidation, and a demo API.

## Why This Project

This project demonstrates production-style patterns for authentication integration:

- Clean abstractions (`ITokenManager`, `ITokenStore`)
- Safe token refresh with synchronization to prevent duplicate refresh calls
- Swappable token persistence strategy
- Unit tests for caching and invalidation behavior

## Architecture

- `OAuthTokenManager`: reusable library
- `OAuthTokenManager.DemoApi`: minimal API that consumes the library
- `OAuthTokenManager.Tests`: unit tests

## Tech Stack

- .NET 8
- ASP.NET Core Minimal API
- xUnit

## Getting Started

### Prerequisites

- .NET 8 SDK

### Configure

Edit app settings:

- `src/OAuthTokenManager.DemoApi/appsettings.json`

Set:

- `OAuth:TokenEndpoint`
- `OAuth:ClientId`
- `OAuth:ClientSecret`
- `OAuth:Scope`

### Run the Demo API

```powershell
# From the repository root
dotnet run --project src/OAuthTokenManager.DemoApi
```

Default local URL from launch settings:

- `http://localhost:5151`

## API

### GET /token

Returns a valid access token. Uses cache until token expiration window is reached.

```powershell
curl http://localhost:5151/token
```

Sample response:

```json
{
  "accessToken": "eyJ..."
}
```

### POST /token/invalidate

Clears cached token so the next `GET /token` forces refresh.

```powershell
curl -X POST http://localhost:5151/token/invalidate
```

## Test

```powershell
# From the repository root
dotnet test OAuthTokenManager.sln
```

## Design Notes

- `TokenManager` uses a semaphore for refresh synchronization, avoiding concurrent refresh storms.
- `TokenResponse.IsExpired` includes a 30-second safety buffer to avoid near-expiry failures mid-request.
- `InMemoryTokenStore` is intentionally simple for demonstration and can be replaced by Redis/DB implementations.

## Portfolio Enhancement Ideas

- Add distributed cache token store (Redis)
- Add secret rotation support
- Add resilient HTTP policy (timeouts/retries/circuit breaker)
- Add metrics and health diagnostics
- Publish as a NuGet package
