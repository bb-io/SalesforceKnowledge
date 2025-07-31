using App.Salesforce.Cms.Constants;
using App.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;

namespace App.Salesforce.Cms.Auth.OAuth2;

public class OAuth2TokenService(InvocationContext InvocationContext) : BaseInvocable(InvocationContext), IOAuth2TokenService
{
    private static string? _tokenUrl;

    public Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        _tokenUrl = $"https://{values[CredNames.Domain]}.my.salesforce.com/services/oauth2/token";

        const string grant_type = "authorization_code";
        var redirectUri = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode";
        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grant_type },
            { "client_id", values[CredNames.ClientId] },
            { "client_secret", values[CredNames.ClientSecret] },
            { "redirect_uri", redirectUri },
            { "code", code }
        };

        return RequestToken(bodyParameters, cancellationToken);
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
        => values.TryGetValue(CredNames.ExpiresAt, out var expireValue) &&
           DateTime.UtcNow > DateTime.Parse(expireValue);

    public async Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        const string grantType = "refresh_token";
        _tokenUrl = $"https://{values[CredNames.Domain]}.my.salesforce.com/services/oauth2/token";
        if (!values.TryGetValue(CredNames.RefreshToken, out var refreshToken))
        {
            throw new("No refresh token found, you should update your OAuth app scopes to give Blackbird access to it");
        }

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "client_id", values[CredNames.ClientId] },
            { "client_secret", values[CredNames.ClientSecret] },
            { "refresh_token", refreshToken },
        };

        var result = await RequestToken(bodyParameters, cancellationToken);
        result["refresh_token"] = refreshToken;

        return result;
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    private async Task<Dictionary<string, string>> RequestToken(Dictionary<string, string> bodyParameters,
        CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = new HttpClient();
            using var httpContent = new FormUrlEncodedContent(bodyParameters);
            using var response = await httpClient.PostAsync(_tokenUrl, httpContent, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var bodyParamsLog = string.Join(", ", bodyParameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                var errorMessage = $"[SalesforceKnowledge] Token request failed. Status: {response.StatusCode}";
                
                var errorResponse = JsonConvert.DeserializeObject<SalesforceErrorDto>(responseContent);
                if (errorResponse?.Error != null)
                {
                    errorMessage += $", API Error: {errorResponse.Error} - {errorResponse.ErrorDescription}";
                }
                else
                {
                    errorMessage += $", Response: {responseContent}";
                }
                
                errorMessage += $", Body parameters: {bodyParamsLog}";
                InvocationContext.Logger?.LogError(errorMessage, []);
                
                throw new InvalidOperationException($"Salesforce Token API error: {errorResponse?.Error ?? response.StatusCode.ToString()} - {errorResponse?.ErrorDescription ?? responseContent}");
            }

            var resultDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            if (resultDictionary == null)
            {
                var bodyParamsLog = string.Join(", ", bodyParameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                InvocationContext.Logger?.LogError($"[SalesforceKnowledge] Failed to deserialize response. Response: {responseContent}, Body parameters: {bodyParamsLog}", []);
                throw new InvalidOperationException($"Invalid response content: {responseContent}");
            }

            if (!resultDictionary.TryGetValue(CredNames.IssuedAt, out var issuedAtValue))
            {
                var bodyParamsLog = string.Join(", ", bodyParameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                var responseKeys = string.Join(", ", resultDictionary.Keys);
                InvocationContext.Logger?.LogError($"[SalesforceKnowledge] Missing 'issued_at' key in response. Response: {responseContent}, Available keys: [{responseKeys}], Body parameters: {bodyParamsLog}", []);
                throw new InvalidOperationException($"Missing 'issued_at' key in response. Available keys: [{responseKeys}]");
            }

            var issuedAt = long.Parse(issuedAtValue);
            var expiresAt = DateTimeOffset.FromUnixTimeMilliseconds(issuedAt).AddHours(2).DateTime;
            resultDictionary.Add(CredNames.ExpiresAt, expiresAt.ToString());
            return resultDictionary;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            var bodyParamsLog = string.Join(", ", bodyParameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            InvocationContext.Logger?.LogError($"[SalesforceKnowledge] Error during token request: {ex.Message}, Body parameters: {bodyParamsLog}", []);
            throw new InvalidOperationException($"Failed to request token: {ex.Message}", ex);
        }
    }
}