using App.Salesforce.Cms.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace App.Salesforce.Cms.Api;

public class SalesforceClient : RestClient
{
    public SalesforceClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(
        new RestClientOptions() { ThrowOnAnyError = true, BaseUrl = GetUri(creds) })
    {
    }

    private static Uri GetUri(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var domainName = creds.Get(CredsNames.Domain).Value;
        return new Uri($"https://{domainName}.my.salesforce.com");
    }
}