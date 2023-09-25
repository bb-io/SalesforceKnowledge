using App.Salesforce.Cms.Constants;
using App.Salesforce.Cms.Extensions;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;

namespace App.Salesforce.Cms.Auth.OAuth2;

public class OAuth2AuthorizeService : IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> dict)
    {
        var domainName = dict[CredsNames.Domain];
        var oauthUrl = $"https://{domainName}.my.salesforce.com/services/oauth2/authorize";

        var parameters = new Dictionary<string, string>
        {
            { "client_id", dict[CredsNames.ClientId] },
            { "redirect_uri", "https://sandbox.blackbird.io/api-rest/connections/AuthorizationCode" },
            { "response_type", "code" },
            { "state", dict["state"] },
        };

        return oauthUrl.WithQuery(parameters);
    }
}