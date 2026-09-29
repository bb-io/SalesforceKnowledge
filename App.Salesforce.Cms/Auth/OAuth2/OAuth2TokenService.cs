using App.Salesforce.Cms.Constants;
using Apps.Salesforce.Cms.Models.Utility.Error;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace App.Salesforce.Cms.Auth.OAuth2;

public class OAuth2TokenService(InvocationContext InvocationContext) : BaseInvocable(InvocationContext), IOAuth2TokenService, ITokenRefreshable
{
    private static string? _tokenUrl;
    private const int TokenExpirationHours = 2;

    public async Task<Dictionary<string, string>> RequestToken(string state, string code, Dictionary<string, string> values,
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

        var connectionFingerprint = CreateFingerprint($"{values[CredNames.Domain]}:{values[CredNames.ClientId]}");
        InvocationContext.Logger?.LogInformation(
            $"[SalesforceKnowledge][OAuth] Starting authorization code exchange. " +
            $"Domain: {values[CredNames.Domain]}; ConnectionFingerprint: {connectionFingerprint}", []);

        var result = await RequestToken(bodyParameters, cancellationToken);
        InvocationContext.Logger?.LogInformation(
            $"[SalesforceKnowledge][OAuth] Authorization code exchange succeeded. " +
            $"Domain: {values[CredNames.Domain]}; ConnectionFingerprint: {connectionFingerprint}; " +
            $"IssuedAt: {GetValueOrMissing(result, CredNames.IssuedAt)}; " +
            $"ExpiresAt: {GetValueOrMissing(result, CredNames.ExpiresAt)}; " +
            $"RefreshTokenReturned: {result.ContainsKey(CredNames.RefreshToken)}", []);

        return result;
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
        => values.TryGetValue(CredNames.ExpiresAt, out var expireValue) &&
           DateTime.UtcNow > DateTime.Parse(expireValue);

    public int? GetRefreshTokenExprireInMinutes(Dictionary<string, string> values)
    {
        if (!values.TryGetValue(CredNames.ExpiresAt, out var expireValue))
            return null;

        if (!DateTime.TryParse(expireValue, out var expireDate))
            return null;

        var difference = expireDate - DateTime.UtcNow;

        return (int)difference.TotalMinutes - 5;
    }

    public async Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values,
        CancellationToken cancellationToken)
    {
        const string grantType = "refresh_token";
        _tokenUrl = $"https://{values[CredNames.Domain]}.my.salesforce.com/services/oauth2/token";
        var connectionFingerprint = CreateFingerprint($"{values[CredNames.Domain]}:{values[CredNames.ClientId]}");

        if (!values.TryGetValue(CredNames.RefreshToken, out var refreshToken))
        {
            InvocationContext.Logger?.LogError(
                $"[SalesforceKnowledge][OAuth] Cannot start refresh token flow because no refresh token is stored. " +
                $"Domain: {values[CredNames.Domain]}; ConnectionFingerprint: {connectionFingerprint}", []);
            throw new("No refresh token found, you should update your OAuth app scopes to give Blackbird access to it");
        }

        var localExpiresAt = GetValueOrMissing(values, CredNames.ExpiresAt);
        var minutesUntilExpiry = GetMinutesUntilExpiry(values);
        var previousRefreshTokenFingerprint = CreateFingerprint(refreshToken);
        InvocationContext.Logger?.LogInformation(
            $"[SalesforceKnowledge][OAuth] Starting refresh token flow. " +
            $"Domain: {values[CredNames.Domain]}; ConnectionFingerprint: {connectionFingerprint}; " +
            $"RefreshTokenFingerprint: {previousRefreshTokenFingerprint}; LocalExpiresAt: {localExpiresAt}; " +
            $"MinutesUntilLocalExpiry: {minutesUntilExpiry?.ToString() ?? "unknown"}", []);

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "client_id", values[CredNames.ClientId] },
            { "client_secret", values[CredNames.ClientSecret] },
            { "refresh_token", refreshToken },
        };

        var result = await RequestToken(bodyParameters, cancellationToken);
        var refreshTokenReturned = result.TryGetValue(CredNames.RefreshToken, out var returnedRefreshToken);
        var refreshTokenRotated = refreshTokenReturned && returnedRefreshToken != refreshToken;
        if (!refreshTokenReturned)
        {
            result[CredNames.RefreshToken] = refreshToken;
        }

        var currentRefreshTokenFingerprint = CreateFingerprint(result[CredNames.RefreshToken]);
        InvocationContext.Logger?.LogInformation(
            $"[SalesforceKnowledge][OAuth] Refresh token flow succeeded. " +
            $"Domain: {values[CredNames.Domain]}; ConnectionFingerprint: {connectionFingerprint}; " +
            $"IssuedAt: {GetValueOrMissing(result, CredNames.IssuedAt)}; " +
            $"ExpiresAt: {GetValueOrMissing(result, CredNames.ExpiresAt)}; " +
            $"RefreshTokenReturned: {refreshTokenReturned}; RefreshTokenRotated: {refreshTokenRotated}; " +
            $"PreviousRefreshTokenFingerprint: {previousRefreshTokenFingerprint}; " +
            $"CurrentRefreshTokenFingerprint: {currentRefreshTokenFingerprint}", []);

        return result;
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    private async Task<Dictionary<string, string>> RequestToken(Dictionary<string, string> bodyParameters,
        CancellationToken cancellationToken)
    {
        var grantType = GetValueOrMissing(bodyParameters, "grant_type");
        var requestFingerprint = CreateFingerprint(
            $"{_tokenUrl}:{GetValueOrMissing(bodyParameters, "client_id")}");

        try
        {
            using var httpClient = new HttpClient();
            using var httpContent = new FormUrlEncodedContent(bodyParameters);
            using var response = await httpClient.PostAsync(_tokenUrl, httpContent, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                AuthError? errorResponse = null;
                var responseFormat = "NonJson";
                try
                {
                    errorResponse = JsonConvert.DeserializeObject<AuthError>(responseContent);
                    responseFormat = "Json";
                }
                catch (JsonException)
                {
                    // Salesforce and upstream gateways can return HTML for service failures.
                }

                InvocationContext.Logger?.LogError(
                    $"[SalesforceKnowledge][OAuth] Token request failed. GrantType: {grantType}; " +
                    $"RequestFingerprint: {requestFingerprint}; StatusCode: {(int)response.StatusCode} ({response.StatusCode}); " +
                    $"ContentType: {response.ContentType ?? "unknown"}; ResponseFormat: {responseFormat}; " +
                    $"ResponseLength: {responseContent.Length}; OAuthError: {errorResponse?.Error ?? "unknown"}; " +
                    $"OAuthErrorDescription: {errorResponse?.ErrorDescription ?? "unavailable"}", []);

                var failureReason = errorResponse?.Error != null
                    ? $"{errorResponse.Error} - {errorResponse.ErrorDescription}"
                    : $"{response.StatusCode} - non-JSON or unrecognized response";
                throw new InvalidOperationException($"Salesforce Token API error: {failureReason}");
            }

            var resultDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            if (resultDictionary == null)
            {
                InvocationContext.Logger?.LogError(
                    $"[SalesforceKnowledge][OAuth] Token response deserialized to null. GrantType: {grantType}; " +
                    $"RequestFingerprint: {requestFingerprint}; ContentType: {response.ContentType ?? "unknown"}; " +
                    $"ResponseLength: {responseContent.Length}", []);
                throw new InvalidOperationException("Invalid response content: token response deserialized to null");
            }

            if (!resultDictionary.TryGetValue(CredNames.IssuedAt, out var issuedAtValue))
            {
                var responseKeys = string.Join(", ", resultDictionary.Keys);
                InvocationContext.Logger?.LogError(
                    $"[SalesforceKnowledge][OAuth] Token response is missing 'issued_at'. GrantType: {grantType}; " +
                    $"RequestFingerprint: {requestFingerprint}; AvailableKeys: [{responseKeys}]", []);
                throw new InvalidOperationException($"Missing 'issued_at' key in response. Available keys: [{responseKeys}]");
            }

            var issuedAt = long.Parse(issuedAtValue);
            var expiresAt = DateTimeOffset.FromUnixTimeMilliseconds(issuedAt).AddHours(TokenExpirationHours).DateTime;
            resultDictionary.Add(CredNames.ExpiresAt, expiresAt.ToString());
            return resultDictionary;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            InvocationContext.Logger?.LogError(
                $"[SalesforceKnowledge][OAuth] Unexpected error during token request. GrantType: {grantType}; " +
                $"RequestFingerprint: {requestFingerprint}; ExceptionType: {ex.GetType().Name}; Message: {ex.Message}", []);
            throw new InvalidOperationException($"Failed to request token: {ex.Message}", ex);
        }
    }

    private static string GetValueOrMissing(IReadOnlyDictionary<string, string> values, string key)
        => values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : "missing";

    private static int? GetMinutesUntilExpiry(IReadOnlyDictionary<string, string> values)
    {
        if (!values.TryGetValue(CredNames.ExpiresAt, out var expiresAt) ||
            !DateTime.TryParse(expiresAt, out var expiryDate))
        {
            return null;
        }

        return (int)(expiryDate - DateTime.UtcNow).TotalMinutes;
    }

    private static string CreateFingerprint(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash)[..12];
    }
}
