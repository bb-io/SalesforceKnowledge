using App.Salesforce.Cms.Api;
using App.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace App.Salesforce.Cms.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        try
        {
            var client = new SalesforceClient(authProviders);

            var endpoint = "/services/data/v57.0/knowledgeManagement/settings";
            var request = new SalesforceRequest(endpoint, Method.Get, authProviders);

            await client.ExecuteAsync(request, cancellationToken);

            return new()
            {
                IsValid = true
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
    }
}