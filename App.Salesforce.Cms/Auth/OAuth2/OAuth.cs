using System;
using System.Text.Json;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Microsoft.AspNetCore.WebUtilities;

namespace Apps.Salesforce.Cms.Auth.OAuth2
{
    public class OAuth2AuthorizeService : IOAuth2AuthorizeService
    {
        public string GetAuthorizationUrl(Dictionary<string, string> dict)
        {
            var domainName = dict["domainName"];
            var oauthUrl = $"https://{domainName}.my.salesforce.com/services/oauth2/authorize";
            //var oauthUrl = "https://login.salesforce.com/services/oauth2/authorize";
            var parameters = new Dictionary<string, string>
            {
                { "client_id", dict["clientId"] },
                { "redirect_uri", "https://sandbox.blackbird.io/api-rest/connections/AuthorizationCode" },
                { "response_type", "code"},
                { "state", dict["state"] },
            };
            return QueryHelpers.AddQueryString(oauthUrl, parameters);
        }
    }

    public class OAuth2TokenService : IOAuth2TokenService
    {
        private const string ExpiresAtKeyName = "expires_at";

        private static string TokenUrl = "";

        public async Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values, CancellationToken cancellationToken)
        {

            TokenUrl = $"https://{values["domainName"]}.my.salesforce.com/services/oauth2/token";
            const string grant_type = "authorization_code";
            const string redirectUri = "https://sandbox.blackbird.io/api-rest/connections/AuthorizationCode";

            var bodyParameters = new Dictionary<string, string>
            {
                { "grant_type", grant_type },
                { "client_id", values["clientId"] },
                { "client_secret", values["clientSecret"] },
                { "redirect_uri", redirectUri },
                { "code", code }
            };
            return await RequestToken(bodyParameters, cancellationToken);
        }

        private async Task<Dictionary<string, string>> RequestToken(Dictionary<string, string> bodyParameters, CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;
            using HttpClient httpClient = new HttpClient();
            using var httpContent = new FormUrlEncodedContent(bodyParameters);
            using var response = await httpClient.PostAsync(TokenUrl, httpContent, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            var resultDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent)?.ToDictionary(r => r.Key, r => r.Value?.ToString())
                ?? throw new InvalidOperationException($"Invalid response content: {responseContent}");
            return resultDictionary;
        }

        public bool IsRefreshToken(Dictionary<string, string> values)
        {
            var expiresAt = DateTime.Parse(values[ExpiresAtKeyName]);
            return DateTime.UtcNow > expiresAt;
        }

        public async Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values, CancellationToken cancellationToken)
        {
            const string grant_type = "refresh_token";

            var bodyParameters = new Dictionary<string, string>
            {
                { "grant_type", grant_type },
                { "client_id", values["clientId"] },
                { "client_secret", values["clientSecret"] },
                { "refresh_token", values["refresh_token"] },
            };
            return await RequestToken(bodyParameters, cancellationToken);
        }

        public Task RevokeToken(Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }

    }
}

