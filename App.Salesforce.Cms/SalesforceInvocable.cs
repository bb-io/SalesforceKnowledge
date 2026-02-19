using App.Salesforce.Cms.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Salesforce.Cms;

public class SalesforceInvocable : BaseInvocable
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    protected SalesforceClient Client { get; }

    protected SalesforceInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
    }
}