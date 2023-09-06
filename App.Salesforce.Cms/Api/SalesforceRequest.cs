using App.Salesforce.Cms.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace App.Salesforce.Cms.Api;

public class SalesforceRequest : RestRequest
{
    public SalesforceRequest(string endpoint, Method method, IEnumerable<AuthenticationCredentialsProvider> creds) :
        base(endpoint, method)
    {
        var token = creds.Get(CredsNames.AccessToken).Value;
        this.AddHeader("Authorization", $"Bearer {token}");
    }

    public void AddLocaleHeader(string locale)
    {
        this.AddHeader("Accept-language", locale.ToLanguageHeaderFormat());
    }
}