using RestSharp;
using Apps.Salesforce.Cms.Api;
using Apps.Salesforce.Cms.Models.Dtos;
using Apps.Salesforce.Cms.Models.Utility.Wrappers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salesforce.Cms.Handlers;

public class ArticleDataHandler(InvocationContext context) : SalesforceInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var query = """            
            SELECT 
                KnowledgeArticleId,
                Title, 
                PublishStatus,
                Language
            FROM Knowledge__kav 
            WHERE IsLatestVersion = true AND IsDeleted = false
            """;

        if (!string.IsNullOrWhiteSpace(context.SearchString))
        {
            var safeSearchString = context.SearchString.Replace("'", "\\'");
            query += $" AND Title LIKE '%{safeSearchString}%'";
        }

        var endpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(query)}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<RecordWrapper<ArticleVersionDto>>(request);
        var grouped = response.Records.DistinctBy(x => x.KnowledgeArticleId);

        return grouped.Select(x => new DataSourceItem(x.KnowledgeArticleId, x.Title)).ToList();
    }
}
