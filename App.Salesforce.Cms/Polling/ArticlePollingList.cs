using App.Salesforce.Cms.Actions.Base;
using App.Salesforce.Cms.Api;
using App.Salesforce.Cms.Models.Responses;
using Apps.Salesforce.Cms.Polling.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Salesforce.Cms.Polling
{
    [PollingEventList]
    public class ArticlePollingList : SalesforceActions
    {
        public ArticlePollingList(InvocationContext invocationContext) : base(invocationContext) {}


        [PollingEvent("On articles created", "On new articles created")]
        public async Task<PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>> OnArticlesCreated(
                   PollingEventRequest<DateMemory> request)
        {
            var query = "SELECT FIELDS(ALL) FROM KnowledgeArticle LIMIT 200";
            var endpoint = $"services/data/v57.0/query?q={query}";

            var articles = await GetArticles(endpoint);

            if (articles.Length == 0)
            {
                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
                };
            }

            if (request.Memory == null)
            {
                var maxCreatedDate = articles.Max(a => DateTime.Parse(a.CreatedDate, null, DateTimeStyles.AdjustToUniversal));
                var memory = new DateMemory { LastInteractionDate = maxCreatedDate };
                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = false,
                    Memory = memory
                };
            }

            var newArticles = articles
                .Where(a => DateTime.Parse(a.CreatedDate, null, DateTimeStyles.AdjustToUniversal) > request.Memory.LastInteractionDate)
                .ToArray();

            if (newArticles.Any())
            {
                var maxCreatedDate = newArticles.Max(a => DateTime.Parse(a.CreatedDate, null, DateTimeStyles.AdjustToUniversal));
                request.Memory.LastInteractionDate = maxCreatedDate;

                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = new ListAllArticlesPollingResponse(newArticles)
                };
            }
            else
            {
                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }
        }

        [PollingEvent("On articles updated", "On any articles updated")]
        public async Task<PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>> OnArticlesUpdated(
            PollingEventRequest<DateMemory> request)
        {
            var query = "SELECT FIELDS(ALL) FROM KnowledgeArticle LIMIT 200";
            var endpoint = $"services/data/v57.0/query?q={query}";

            var articles = await GetArticles(endpoint);

            if (articles.Length == 0)
            {
                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
                };
            }

            if (request.Memory == null)
            {
                var maxUpdatedDate = articles.Max(a => DateTime.Parse(a.LastModifiedDate, null, DateTimeStyles.AdjustToUniversal));
                var memory = new DateMemory { LastInteractionDate = maxUpdatedDate };
                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = false,
                    Memory = memory
                };
            }

            var updatedArticles = articles
                .Where(a => DateTime.Parse(a.LastModifiedDate, null, DateTimeStyles.AdjustToUniversal) > request.Memory.LastInteractionDate)
                .ToArray();

            if (updatedArticles.Any())
            {
                var maxUpdatedDate = updatedArticles.Max(a => DateTime.Parse(a.LastModifiedDate, null, DateTimeStyles.AdjustToUniversal));
                request.Memory.LastInteractionDate = maxUpdatedDate;

                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = true,
                    Memory = request.Memory,
                    Result = new ListAllArticlesPollingResponse(updatedArticles)
                };
            }
            else
            {
                return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory
                };
            }
        }

        private async Task<KnowledgeArticle[]> GetArticles(string endpoint)
        {
            var response = await Client.ExecuteWithErrorHandling<ListAllArticlesPollingResponse>(
            new SalesforceRequest(endpoint, Method.Get, Creds));

            return response.Records;
        }
    }
}
