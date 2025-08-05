using App.Salesforce.Cms.Constants;
using Apps.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace App.Salesforce.Cms.Api;

public class SalesforceClient(IEnumerable<AuthenticationCredentialsProvider> creds) : BlackBirdRestClient(
    new RestClientOptions
    {
        ThrowOnAnyError = false,
        BaseUrl = GetUri(creds)
    })
{
    private static Uri GetUri(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var domainName = creds.Get(CredNames.Domain).Value;
        return new Uri($"https://{domainName}.my.salesforce.com");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if (string.IsNullOrEmpty(response.Content))
        {
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new PluginApplicationException($"Status code: {response.StatusCode}, but no content or error message provided.");
            }

            return new PluginApplicationException(response.ErrorMessage);
        }

        var errorDto = JsonConvert.DeserializeObject<List<ErrorDto>>(response.Content!);
        if (errorDto == null || errorDto.Count == 0)
        {
            return new PluginApplicationException($"Status code: {response.StatusCode}, {response.Content}");
        }

        var errorMessage = string.Join(", ", errorDto.ConvertAll(e => $"{e.Message} (Code: {e.ErrorCode})"));
        return new PluginApplicationException($"Status code: {response.StatusCode}, {errorMessage}");
    }
}