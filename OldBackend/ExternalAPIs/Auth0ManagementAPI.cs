using Backend.Models.ManagementApi;
using BugTrackerBackend.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BugTrackerBackend.ExternalAPIs;

public class Auth0ManagementAPI(Auth0Options options, IHttpClientFactory httpClientFactory)
{
    readonly Auth0Options _options = options;
    readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    string? _accessToken = null;

    public async Task<string?> GetAccessTokenAsync()
    {
        using HttpClient client = _httpClientFactory.CreateClient();
        Dictionary<string, string> formContent = new()
        {
            { "grant_type", "client_credentials" },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
            { "audience", $"https://{_options.Domain}/api/v2/" }
        };

        ConfigurationManager<OpenIdConnectConfiguration> configurationManager = new(
            $"https://{_options.Domain}/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        OpenIdConnectConfiguration discoveryDocument = await configurationManager.GetConfigurationAsync();

        TokenValidationParameters validationParameters = new()
        {
            ValidateAudience = true,
            ValidAudience = $"https://{_options.Domain}/api/v2/",
            ValidateIssuer = true,
            ValidIssuer = $"https://{_options.Domain}/",
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = discoveryDocument.SigningKeys,
            ValidateLifetime = true
        };

        JsonWebTokenHandler tokenHandler = new();
        TokenValidationResult result = await tokenHandler.ValidateTokenAsync(_accessToken, validationParameters);
        if (result.IsValid) return _accessToken;

        try
        {
            HttpResponseMessage response = await client.PostAsync($"https://{_options.Domain}/oauth/token/", new FormUrlEncodedContent(formContent));
            TokenResponse? tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse is not null) _accessToken = tokenResponse.AccessToken;
        }
        catch
        {
            _accessToken = null;
        }

        return _accessToken;
    }
}
