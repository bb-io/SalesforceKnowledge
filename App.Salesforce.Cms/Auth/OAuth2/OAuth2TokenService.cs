using App.Salesforce.Cms.Constants;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Newtonsoft.Json;

namespace App.Salesforce.Cms.Auth.OAuth2;

public class OAuth2TokenService : IOAuth2TokenService
{
    private static string? _tokenUrl;

    public Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        _tokenUrl = $"https://{values[CredsNames.Domain]}.my.salesforce.com/services/oauth2/token";
        const string grant_type = "authorization_code";
        const string redirectUri = "https://dev.blackbird.io/api-rest/connections/AuthorizationCode";

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grant_type },
            { "client_id", values[CredsNames.ClientId] },
            { "client_secret", values[CredsNames.ClientSecret] },
            { "redirect_uri", redirectUri },
            { "code", code }
        };

        return RequestToken(bodyParameters, cancellationToken);
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
        => values.TryGetValue(CredsNames.ExpiresAt, out var expireValue) &&
           DateTime.UtcNow > DateTime.Parse(expireValue);

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        const string grant_type = "refresh_token";

        _tokenUrl = $"https://{values[CredsNames.Domain]}.my.salesforce.com/services/oauth2/token";

        if (!values.TryGetValue(CredsNames.RefreshToken, out var refreshToken))
            throw new("No refresh token found, you should update your OAuth app scopes to give Blackbird access to it");

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grant_type },
            { "client_id", values[CredsNames.ClientId] },
            { "client_secret", values[CredsNames.ClientSecret] },
            { "refresh_token", refreshToken },
        };

        return RequestToken(bodyParameters, cancellationToken);
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    private async Task<Dictionary<string, string>> RequestToken(Dictionary<string, string> bodyParameters,
        CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        using var httpContent = new FormUrlEncodedContent(bodyParameters);
        using var response = await httpClient.PostAsync(_tokenUrl, httpContent, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();
        var resultDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent)
                               ?? throw new InvalidOperationException($"Invalid response content: {responseContent}");

        var issuedAt = long.Parse(resultDictionary[CredsNames.IssuedAt]);
        var expiresAt = DateTimeOffset.FromUnixTimeMilliseconds(issuedAt).AddHours(2).DateTime;
        resultDictionary.Add(CredsNames.ExpiresAt, expiresAt.ToString());

        return resultDictionary;
    }
}