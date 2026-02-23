using RestSharp;
using Apps.Salesforce.Cms.Api;
using Apps.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salesforce.Cms.Handlers;

public class LocaleDataHandler(InvocationContext context) : SalesforceInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var endpoint = "/services/data/v57.0/knowledgeManagement/settings";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<KnowledgeSettingsDto>(request);

        var languages = response.Languages.Where(x => x.Active == true).Select(x => x.Name);         
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            languages = languages.Where(l => l.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));

        return languages.Select(x => new DataSourceItem(x, x));
    }
}
