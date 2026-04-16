using System.Net;
using System.Text;
using System.Text.Json;
using OAuthTokenManager.Models;
using OAuthTokenManager.Services;
using Xunit;

namespace OAuthTokenManager.Tests;

public class TokenManagerInvalidateTests
{
    [Fact]
    public async Task InvalidateTokenAsync_ClearsCacheAndRequestsNewToken()
    {
        var call = 0;
        var handler = new FakeHttpMessageHandler(_ =>
        {
            call++;
            var token = call == 1 ? "first" : "second";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new { access_token = token, token_type = "Bearer", expires_in = 3600 }),
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var client = new HttpClient(handler);
        var tokenStore = new InMemoryTokenStore();
        var request = new TokenRequest("https://example.com/oauth/token", "client", "secret");
        var manager = new Services.TokenManager(client, tokenStore, request);

        var first = await manager.GetAccessTokenAsync();
        await manager.InvalidateTokenAsync();
        var second = await manager.GetAccessTokenAsync();

        Assert.Equal("first", first);
        Assert.Equal("second", second);
        Assert.Equal(2, handler.CallCount);
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
