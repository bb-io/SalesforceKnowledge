using App.Salesforce.Cms.Constants;
using App.Salesforce.Cms.Extensions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace App.Salesforce.Cms.Auth.OAuth2;

public class OAuth2AuthorizeService(InvocationContext InvocationContext) : BaseInvocable(InvocationContext), IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> dict)
    {
        var domainName = dict[CredNames.Domain];
        var oauthUrl = $"https://{domainName}.my.salesforce.com/services/oauth2/authorize";
        var bridgeOauthUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/oauth";

        var parameters = new Dictionary<string, string>
        {
            { "client_id", dict[CredNames.ClientId] },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "actual_redirect_uri", InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString() },
            { "response_type", "code" },
            { "state", dict["state"] },
            { "authorization_url", oauthUrl}
        };

        return bridgeOauthUrl.WithQuery(parameters);
    }
}