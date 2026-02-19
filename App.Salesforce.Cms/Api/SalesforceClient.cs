using App.Salesforce.Cms.Constants;
using Apps.Salesforce.Cms.Models.Utility.Error;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Salesforce.Cms.Api;

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

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        string content = (await ExecuteWithErrorHandling(request)).Content;
        T val = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (val == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return val;
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        RestResponse restResponse = await ExecuteAsync(request);
        if (!restResponse.IsSuccessStatusCode)
        {
            throw ConfigureErrorException(restResponse);
        }

        return restResponse;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var content = response.Content;
        if (string.IsNullOrEmpty(content))
        {
            return new PluginApplicationException(
                $"Status code: {response.StatusCode}, but no content or error message provided."
            );
        }

        if (content.TrimStart().StartsWith('['))
        {
            var errors = JsonConvert.DeserializeObject<List<ErrorResponse>>(content);
            if (errors != null && errors.Count != 0)
            {
                var errorMessages = errors.SelectMany(x => x.Errors);
                string msg = string.Join("; ", errorMessages.Select(x => x.Message));
                return new PluginApplicationException($"Status code: {response.StatusCode}. {msg}");
            }
        }
        else
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
            if (errorResponse?.Errors != null && errorResponse.Errors.Count != 0)
            {
                string msg = string.Join("; ", errorResponse.Errors.Select(x => x.Message));
                return new PluginApplicationException($"Status code: {response.StatusCode}. {msg}");
            }
        }

        return new PluginApplicationException($"Status code: {response.StatusCode}, {content}");
    }
}