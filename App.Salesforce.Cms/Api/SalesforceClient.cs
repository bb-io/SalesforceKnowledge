using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using App.Salesforce.Cms.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace App.Salesforce.Cms.Api
{
    public class SalesforceClient : BlackBirdRestClient
    {
        public SalesforceClient(IEnumerable<AuthenticationCredentialsProvider> creds)
            : base(new RestClientOptions
            {
                ThrowOnAnyError = true,
                BaseUrl = GetUri(creds)
            })
        {
        }

        private static Uri GetUri(IEnumerable<AuthenticationCredentialsProvider> creds)
        {
            var domainName = creds.Get(CredNames.Domain).Value;
            return new Uri($"https://{domainName}.my.salesforce.com");
        }

        protected override Exception ConfigureErrorException(RestResponse response)
        {
            throw new PluginApplicationException($"Salesforce API returned error: {(int)response.StatusCode} - {response.Content}");
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
            RestResponse restResponse;
            try
            {
                 restResponse = await ExecuteAsync(request);
            }
            catch (Exception ex)
            {
                throw new PluginApplicationException($"Salesforce API returned error: {ex.Message}");
            }

            if (!restResponse.IsSuccessStatusCode)
            {
                throw ConfigureErrorException(restResponse);
            }

            return restResponse;
        }
    }
}
