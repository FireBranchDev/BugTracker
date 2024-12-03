using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace BackendApiTests;

public class ApiFixture : IAsyncLifetime
{
    readonly BackendApiTestingWebApplicationFactory<Program> _factory;

    public HttpClient ApiClient { get; set; }
    public HttpClient Auth0Client { get; set; }

    public ApiFixture()
    {
        _factory = new();

        ApiClient = _factory.CreateClient();

        Config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddUserSecrets<ApiFixture>()
            .Build();

        ApiClient.BaseAddress = new Uri($"https://{Config[ConfigKeys.BackendApiDomain]}/api/");

        Auth0Client = new HttpClient();
    }

    public IConfigurationRoot Config { get; private set; }

    string _accessToken = string.Empty;

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken)) return _accessToken;

        string clientId = Config[ConfigKeys.Auth0ClientId]!;
        string clientSecret = Config[ConfigKeys.Auth0ClientSecret]!;
        string backendApiAudience = Config[ConfigKeys.BackendApiAuth0Audience]!;

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new($"https://{Config[ConfigKeys.BackendApiAuth0Domain]}/oauth/token")
        };
        Dictionary<string, string> data = new()
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "audience", backendApiAudience }
        };
        request.Content = new FormUrlEncodedContent(data);

        HttpResponseMessage response = await Auth0Client.SendAsync(request);
        Dictionary<string, JsonElement>? result = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
        if (result is null) return string.Empty;

        _accessToken = result["access_token"].GetString() ?? string.Empty;

        return _accessToken;
    }

    public async Task InitializeAsync()
    {
        ApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
