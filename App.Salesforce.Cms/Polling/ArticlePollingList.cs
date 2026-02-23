using RestSharp;
using Apps.Salesforce.Cms.Api;
using Apps.Salesforce.Cms.Models.Dtos;
using Apps.Salesforce.Cms.Models.Requests;
using Apps.Salesforce.Cms.Models.Responses;
using Apps.Salesforce.Cms.Models.Utility.Wrappers;
using Apps.Salesforce.Cms.Polling.Models;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salesforce.Cms.Polling;

[PollingEventList("Articles")]
public class ArticlePollingList(InvocationContext invocationContext) : SalesforceInvocable(invocationContext)
{
    [PollingEvent("On articles created or updated", "Triggered when articles are created or updated")]
    public async Task<PollingEventResponse<DateMemory, SearchMasterArticlesResponse>> OnArticlesUpdated(
        PollingEventRequest<DateMemory> request)
    {
        if (request.Memory == null)
        {
            return new PollingEventResponse<DateMemory, SearchMasterArticlesResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        string dateFilter = request.Memory.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        string versionQuery = $"""
            SELECT 
                KnowledgeArticleId 
            FROM 
                Knowledge__kav 
            WHERE 
                LastModifiedDate > {dateFilter} AND 
                IsLatestVersion = true
            ORDER BY LastModifiedDate DESC
            LIMIT 200
            """;

        string versionEndpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(versionQuery)}";
        var versionRequest = new SalesforceRequest(versionEndpoint, Method.Get, Creds);
        var versionResponse = await Client.ExecuteWithErrorHandling<RecordWrapper<ArticleVersionDto>>(versionRequest);

        var updatedMasterIds = versionResponse?.Records?
            .Select(v => v.KnowledgeArticleId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList() ?? [];

        if (updatedMasterIds.Count == 0)
        {
            return new PollingEventResponse<DateMemory, SearchMasterArticlesResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        string idsString = string.Join(",", updatedMasterIds.Select(id => $"'{id}'"));
        string masterQuery = $"""
            SELECT 
                FIELDS(ALL) 
            FROM 
                KnowledgeArticle 
            WHERE 
                Id IN ({idsString}) AND 
                IsDeleted = false
            LIMIT 200
            """;

        string masterEndpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(masterQuery)}";
        var masterRequest = new SalesforceRequest(masterEndpoint, Method.Get, Creds);
        var masterResponse = await Client.ExecuteWithErrorHandling<RecordWrapper<MasterArticleDto>>(masterRequest);

        return new PollingEventResponse<DateMemory, SearchMasterArticlesResponse>
        {
            FlyBird = masterResponse.Records.Any(),
            Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
            Result = new SearchMasterArticlesResponse(masterResponse.Records.ToList())
        };
    }

    [PollingEvent("On articles created", "Triggered when articles are created")]
    public async Task<PollingEventResponse<DateMemory, SearchMasterArticlesResponse>> OnArticlesCreated(
        PollingEventRequest<DateMemory> request)
    {
        if (request.Memory == null)
        {
            return new PollingEventResponse<DateMemory, SearchMasterArticlesResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
                Result = new SearchMasterArticlesResponse([])
            };
        }

        string dateFilter = request.Memory.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        string query =
            $"""
            SELECT 
                FIELDS(ALL) 
            FROM KnowledgeArticle 
            WHERE 
                CreatedDate > {dateFilter} AND 
                IsDeleted = false 
            ORDER BY CreatedDate DESC 
            LIMIT 200
            """;
        string endpoint = $"services/data/v57.0/query?q={Uri.EscapeDataString(query)}";

        var masterRequest = new SalesforceRequest(endpoint, Method.Get, Creds);
        var masterResponse = await Client.ExecuteWithErrorHandling<RecordWrapper<MasterArticleDto>>(masterRequest);

        return new PollingEventResponse<DateMemory, SearchMasterArticlesResponse>
        {
            FlyBird = masterResponse.Records.Any(),
            Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
            Result = new SearchMasterArticlesResponse(masterResponse.Records.ToList())
        };
    }

    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
    [PollingEvent("On article published", "Triggered when articles are published")]
    public async Task<PollingEventResponse<DateMemory, SearchArticlesResponse>> OnPublishedArticlesCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] CategoryFilterRequest category,
        [PollingEventParameter] VisibilityFilterRequest visibility)
    {
        if (request.Memory == null)
        {
            return new PollingEventResponse<DateMemory, SearchArticlesResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
                Result = new SearchArticlesResponse([])
            };
        }

        var langEndpoint = "/services/data/v57.0/knowledgeManagement/settings";
        var langRequest = new SalesforceRequest(langEndpoint, Method.Get, Creds);
        var languageDetails = await Client.ExecuteWithErrorHandling<KnowledgeSettingsDto>(langRequest);
        var locale = languageDetails?.DefaultLanguage ?? "en_US";

        var endpoint = "services/data/v57.0/support/knowledgeArticles?pageSize=100&sort=LastPublishedDate";
        var sfRequest = new SalesforceRequest(endpoint, Method.Get, Creds);
        sfRequest.AddLocaleHeader(locale);

        var publishedArticles = await Client.ExecuteWithErrorHandling<PublishedArticlesResponse>(sfRequest);
        var filtered = publishedArticles?.Articles?.Where(a => a.LastPublishedDate > request.Memory.LastInteractionDate) ?? [];
        var recentArticles = filtered.ToList();

        if (recentArticles.Count == 0)
        {
            return new PollingEventResponse<DateMemory, SearchArticlesResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
                Result = new SearchArticlesResponse([])
            };
        }

        var idsForVisibility = recentArticles.Select(a => a.Id).ToArray();
        var visMap = await LoadVisibilityByArticleIdAsync(idsForVisibility, locale);
        var visibilityFiltered = recentArticles.Select(a =>
        {
            if (visMap.TryGetValue(a.Id, out var v))
            {
                a.IsVisibleInPkb = v.IsVisibleInPkb;
                a.IsVisibleInCsp = v.IsVisibleInCsp;
            }
            return a;
        });

        if (visibility?.IsVisibleInPkb is bool wantPkb)
            visibilityFiltered = visibilityFiltered.Where(a => (a.IsVisibleInPkb ?? false) == wantPkb);

        if (visibility?.IsVisibleInCsp is bool wantCsp)
            visibilityFiltered = visibilityFiltered.Where(a => (a.IsVisibleInCsp ?? false) == wantCsp);

        if (!string.IsNullOrEmpty(category?.CategoryName))
        {
            visibilityFiltered = visibilityFiltered.Where(a =>
                a.CategoryGroups?.Any(cg => 
                    cg.SelectedCategories?.Any(sc => sc.CategoryName == category.CategoryName) == true) == true);
        }

        if (!string.IsNullOrEmpty(category?.GroupName))
        {
            visibilityFiltered = visibilityFiltered.Where(a => 
                a.CategoryGroups?.Any(cg => cg.GroupName == category.GroupName) == true);
        }

        if (category?.ExcludedDataCategories?.Any() == true)
        {
            var excludedSet = new HashSet<string>(category.ExcludedDataCategories, StringComparer.OrdinalIgnoreCase);
            visibilityFiltered = visibilityFiltered.Where(a =>
                a.CategoryGroups == null || !a.CategoryGroups.Any(cg =>
                    cg.SelectedCategories?.Any(
                        sc => !string.IsNullOrEmpty(sc.CategoryName) && excludedSet.Contains(sc.CategoryName)) == true));
        }

        if (category?.ExcludedGroupNames?.Any() == true)
        {
            var excludedGroups = new HashSet<string>(category.ExcludedGroupNames, StringComparer.OrdinalIgnoreCase);
            visibilityFiltered = visibilityFiltered.Where(a =>
                a.CategoryGroups == null || !a.CategoryGroups.Any(cg =>
                    !string.IsNullOrEmpty(cg.GroupName) && excludedGroups.Contains(cg.GroupName)));
        }

        var records = visibilityFiltered
            .Select(a => new ArticleDto(a, locale))
            .ToList();

        return new PollingEventResponse<DateMemory, SearchArticlesResponse>
        {
            FlyBird = records.Count != 0,
            Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
            Result = new SearchArticlesResponse(records)
        };
    }

    private async Task<Dictionary<string, KnowledgeArticleVisibilityDto>> LoadVisibilityByArticleIdAsync(
        IEnumerable<string> articleIds, 
        string locale)
    {
        var ids = articleIds.Distinct().ToArray();
        if (ids.Length == 0) 
            return [];

        string q1 =
            $"SELECT KnowledgeArticleId, Id, IsVisibleInPkb, IsVisibleInCsp " +
            $"FROM KnowledgeArticleVersion " +
            $"WHERE PublishStatus = 'Online' AND Language = '{locale}' " +
            $"AND KnowledgeArticleId IN ({string.Join(",", ids.Select(x => $"'{x}'"))})";

        var req = new SalesforceRequest("services/data/v57.0/query", Method.Get, Creds);
        req.AddQueryParameter("q", q1);
        req.AddLocaleHeader(locale);

        var r1 = await Client.ExecuteWithErrorHandling<RecordWrapper<KnowledgeArticleVisibilityDto>>(req);
        var map = r1?.Records?.ToDictionary(x => x.KnowledgeArticleId, x => x) ?? [];

        if (map.Count == 0)
        {
            string q2 =
                $"SELECT KnowledgeArticleId, Id, IsVisibleInPkb, IsVisibleInCsp " +
                $"FROM KnowledgeArticleVersion " +
                $"WHERE PublishStatus = 'Online' AND Language = '{locale}' " +
                $"AND Id IN ({string.Join(",", ids.Select(x => $"'{x}'"))})";

            var req2 = new SalesforceRequest("services/data/v57.0/query", Method.Get, Creds);
            req2.AddQueryParameter("q", q2);
            req2.AddLocaleHeader(locale);

            var r2 = await Client.ExecuteWithErrorHandling<RecordWrapper<KnowledgeArticleVisibilityDto>>(req2);
            map = r2?.Records?.GroupBy(x => x.KnowledgeArticleId).ToDictionary(g => g.Key, g => g.First()) ?? [];
        }

        return map;
    }
}
