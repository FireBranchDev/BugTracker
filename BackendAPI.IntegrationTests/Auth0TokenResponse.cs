using System.Text.Json.Serialization;

namespace BackendAPI.IntegrationTests;

public class Auth0TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}
