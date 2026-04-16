using System.Net;
using System.Text;
using OAuthTokenManager.Models;
using OAuthTokenManager.Services;
using Xunit;

namespace OAuthTokenManager.Tests;

public class TokenManagerTests
{
    [Fact]
    public async Task GetAccessTokenAsync_CachesTokenUntilExpired()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"access_token\":\"abc123\",\"token_type\":\"Bearer\",\"expires_in\":3600}",
                    Encoding.UTF8,
                    "application/json")
            });

        var client = new HttpClient(handler);
        var tokenStore = new InMemoryTokenStore();
        var request = new TokenRequest("https://example.com/oauth/token", "client", "secret", "scope");

        var manager = new Services.TokenManager(client, tokenStore, request);

        var first = await manager.GetAccessTokenAsync();
        var second = await manager.GetAccessTokenAsync();

        Assert.Equal("abc123", first);
        Assert.Equal("abc123", second);
        Assert.Equal(1, handler.CallCount);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public int CallCount { get; private set; }

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(_responseFactory(request));
        }
    }
}
