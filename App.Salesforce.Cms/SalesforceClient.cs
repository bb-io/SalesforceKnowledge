using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;

namespace Apps.Salesforce.Cms;

public class SalesforceClient : RestClient
{

    public SalesforceClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base(new RestClientOptions() { ThrowOnAnyError = true, BaseUrl = GetUri(authenticationCredentialsProviders) }) { }
    private static Uri GetUri(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider)
    {
        var domainName = authenticationCredentialsProvider.First(v => v.KeyName == "domainName").Value;
        return new Uri($"https://{domainName}.my.salesforce.com");

    }
}