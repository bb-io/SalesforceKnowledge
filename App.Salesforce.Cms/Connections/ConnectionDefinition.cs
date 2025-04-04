using App.Salesforce.Cms.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace App.Salesforce.Cms.Connections;

public class OAuth2ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups =>
    [
        new()
        {
            Name = "OAuth2",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties =
            [
                new(CredNames.Domain)
                {
                    DisplayName = "Domain name"
                },
                new(CredNames.ClientId)
                {
                    DisplayName = "Client ID"
                },
                new(CredNames.ClientSecret)
                {
                    DisplayName = "Client secret",
                    Sensitive = true 
                }
            ]
        }
    ];

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        if (!values.ContainsKey(CredNames.AccessToken))
        {
            throw new Exception("Access token not found");
        }

        if (!values.ContainsKey(CredNames.Domain))
        {
            throw new Exception("Domain name not found");
        }

        return values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value));
    }
}