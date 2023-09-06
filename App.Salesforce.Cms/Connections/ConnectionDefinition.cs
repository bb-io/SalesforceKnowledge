using App.Salesforce.Cms.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace App.Salesforce.Cms.Connections;

public class OAuth2ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "OAuth2",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>()
            {
                new(CredsNames.Domain) { DisplayName = "Domain name" },
                new(CredsNames.ClientId) { DisplayName = "Client ID" },
                new(CredsNames.ClientSecret) { DisplayName = "Client secret" }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        if(!values.ContainsKey(CredsNames.AccessToken))
            throw new Exception("Access token not found");
        
        if(!values.ContainsKey(CredsNames.Domain))
            throw new Exception("Domain name not found");

        return values.Select(x =>
            new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.None,
                x.Key,
                x.Value));
    }
}