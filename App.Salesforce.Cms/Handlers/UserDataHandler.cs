using RestSharp;
using Apps.Salesforce.Cms.Api;
using Apps.Salesforce.Cms.Models.Dtos;
using Apps.Salesforce.Cms.Models.Utility.Wrappers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salesforce.Cms.Handlers;

public class UserDataHandler(InvocationContext context) : SalesforceInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var query = 
            """
            SELECT 
                Id, 
                Name, 
                Email 
            FROM User 
            WHERE 
                IsActive = true AND 
                UserType = 'Standard' AND
                (NOT Email LIKE 'noreply@%')
            """;

        if (!string.IsNullOrWhiteSpace(context.SearchString))
        {
            var safeSearch = context.SearchString.Replace("'", "\\'");
            query += $" AND (Name LIKE '%{safeSearch}%' OR Email LIKE '%{safeSearch}%')";
        }

        query += " ORDER BY Name ASC LIMIT 200";

        var endpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(query)}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<RecordWrapper<UserDto>>(request);
        return response.Records.Select(x => new DataSourceItem(x.Id, $"{x.Name} ({x.Email})")).ToList();
    }
}
