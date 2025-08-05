using App.Salesforce.Cms.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace App.Salesforce.Cms.Actions.Base;

public class SalesforceActions : BaseInvocable
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    protected SalesforceClient Client { get; }

    protected SalesforceActions(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
    }
}