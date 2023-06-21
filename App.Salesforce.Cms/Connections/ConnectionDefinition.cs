using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Salesforce.Cms.Connections
{
    public class OAuth2ConnectionDefinition : IConnectionDefinition
    {
        public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
        {
            new ConnectionPropertyGroup
            {
                Name = "OAuth2",
                AuthenticationType = ConnectionAuthenticationType.OAuth2,
                ConnectionUsage = ConnectionUsage.Actions,
                ConnectionProperties = new List<ConnectionProperty>()
                {
                    new ConnectionProperty("domainName"),
                    new ConnectionProperty("clientId"),
                    new ConnectionProperty("clientSecret")
                }

            }
        };

        public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
        {
            string token;
            string domainName;
            try
            {
                token = values.First(v => v.Key == "access_token").Value;
            } catch
            {
                throw new Exception("access_token not found");
            }

            try
            {
                domainName = values.First(v => v.Key == "domainName").Value;
            } catch
            {
                throw new Exception("domain name not found");
            }
            
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.Header,
                "Authorization",
                $"Bearer {token}"
            );

            
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.QueryString,
                "domainName",
                domainName
             );
        }
    }
}