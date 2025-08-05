using App.Salesforce.Cms.Actions.Base;
using App.Salesforce.Cms.Api;
using Apps.Salesforce.Cms.Models.Dtos;
using Apps.Salesforce.Cms.Polling.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Salesforce.Cms.Polling;

[PollingEventList]
public class ArticlePollingList(InvocationContext invocationContext) : SalesforceActions(invocationContext)
{
    [PollingEvent("On articles created", "Polling event, that periodically checks for new articles created in Salesforce.")]
    public async Task<PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>> OnArticlesCreated(
               PollingEventRequest<DateMemory> request)
    {
        if (request.Memory == null)
        {
            return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
                Result = new ListAllArticlesPollingResponse(Array.Empty<MasterArticleDto>())
            };
        }

        var dateFilter = request.Memory.LastInteractionDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
        var query = $"SELECT FIELDS(ALL) FROM KnowledgeArticle WHERE CreatedDate > {dateFilter} ORDER BY CreatedDate DESC LIMIT 200";
        var endpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(query)}";

        var articles = await GetArticles(endpoint);
        return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
        {
            FlyBird = articles.Length > 0,
            Memory = new DateMemory
            {
                LastInteractionDate = DateTime.UtcNow
            },
            Result = new ListAllArticlesPollingResponse(articles)
        };
    }

    [PollingEvent("On articles updated", "Polling event, that periodically checks for updated articles in Salesforce.")]
    public async Task<PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>> OnArticlesUpdated(
        PollingEventRequest<DateMemory> request)
    {
        if (request.Memory == null)
        {
            return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        var dateFilter = request.Memory.LastInteractionDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
        var query = $"SELECT FIELDS(ALL) FROM KnowledgeArticle WHERE LastModifiedDate > {dateFilter} ORDER BY LastModifiedDate DESC LIMIT 200";
        var endpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(query)}";

        var articles = await GetArticles(endpoint);
        return new PollingEventResponse<DateMemory, ListAllArticlesPollingResponse>
        {
            FlyBird = articles.Length > 0,
            Memory = new DateMemory
            {
                LastInteractionDate = DateTime.UtcNow
            },
            Result = new ListAllArticlesPollingResponse(articles)
        };
    }

    private async Task<MasterArticleDto[]> GetArticles(string endpoint)
    {
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ListAllArticlesPollingResponse>(request);
        return response.Records.Where(x => x.IsDeleted == false).ToArray();
    }
}
