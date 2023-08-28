using App.Salesforce.Cms;
using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;

namespace Apps.Salesforce.Cms;

public class SalesforceRequest : RestRequest
{
    public SalesforceRequest(string endpoint, Method method, IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base(endpoint, method)
    {
        var token = authenticationCredentialsProviders.First(p => p.KeyName == "Authorization").Value;
        this.AddHeader("Authorization", $"{token}");
    }

    public void AddLocaleHeader(string locale)
    {
        this.AddHeader("Accept-language", locale.ToLanguageHeaderFormat());
    }
}