using App.Salesforce.Cms.Constants;
using Apps.Salesforce.Cms.Models.Utility.Error;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        string? content = response.Content;
        if (string.IsNullOrEmpty(content))
        {
            return new PluginApplicationException(
                $"Status code: {response.StatusCode}, but no content or error message provided."
            );
        }
        if (response.ContentType == "text/html")
            return new PluginApplicationException($"Status code: {response.StatusCode}. HTML Error: {content}");

        try
        {
            var jsonToken = JToken.Parse(content);
            var extractedMessages = new List<string>();

            if (jsonToken is JArray arrayToken && arrayToken.Count > 0)
            {
                var firstItem = arrayToken.First as JObject;

                if (firstItem != null && firstItem.ContainsKey("errors"))
                {
                    var actionErrors = arrayToken.ToObject<List<ErrorResponse>>();
                    if (actionErrors != null)
                        extractedMessages.AddRange(actionErrors.SelectMany(x => x.Errors).Select(e => e.Message));
                }
                else if (firstItem != null && firstItem.ContainsKey("message"))
                {
                    var standardErrors = arrayToken.ToObject<List<Error>>();
                    if (standardErrors != null)
                        extractedMessages.AddRange(standardErrors.Select(e => e.Message));
                }
            }
            else if (jsonToken is JObject objToken)
            {
                if (objToken.ContainsKey("errors"))
                {
                    var errorResponse = objToken.ToObject<ErrorResponse>();
                    if (errorResponse?.Errors != null)
                        extractedMessages.AddRange(errorResponse.Errors.Select(e => e.Message));
                }
                else if (objToken.ContainsKey("message"))
                {
                    var singleError = objToken.ToObject<Error>();
                    if (singleError != null)
                        extractedMessages.Add(singleError.Message);
                }
            }

            if (extractedMessages.Count != 0)
            {
                string finalMessage = string.Join("; ", extractedMessages.Where(m => !string.IsNullOrWhiteSpace(m)));
                return new PluginApplicationException($"Status code: {response.StatusCode}. {finalMessage}");
            }
        }
        catch (JsonReaderException) 
        {
            return new PluginApplicationException($"Status code: {response.StatusCode}. Unable to parse the error");
        }

        return new PluginApplicationException($"Status code: {response.StatusCode}. Raw error: {content}");
    }
}