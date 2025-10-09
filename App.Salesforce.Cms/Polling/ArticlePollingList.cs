using App.Salesforce.Cms.Actions.Base;
using App.Salesforce.Cms.Api;
using App.Salesforce.Cms.Models.Dtos;
using App.Salesforce.Cms.Models.Responses;
using Apps.Salesforce.Cms.Models.Dtos;
using Apps.Salesforce.Cms.Models.Requests;
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

    [PollingEvent("On published articles last published", "Polling event, that periodically checks for  published articles in Salesforce.")]
    public async Task<PollingEventResponse<DateMemory, ListAllArticlesResponse>> OnPublishedArticlesCreated(
    PollingEventRequest<DateMemory> request,[PollingEventParameter] CategoryFilterRequest category,
                                            [PollingEventParameter] VisibilityFilterRequest visibility)
    {
        var langEndpoint = "/services/data/v57.0/knowledgeManagement/settings";
        var lang = new SalesforceRequest(langEndpoint, Method.Get, Creds);

        var languageDetails =await Client.ExecuteWithErrorHandling<KnowledgeSettingsDto>(lang)!;
        var locale = languageDetails.DefaultLanguage;

        if (request.Memory == null)
        {
            return new PollingEventResponse<DateMemory, ListAllArticlesResponse>
            {
                FlyBird = false,
                Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
                Result = new ListAllArticlesResponse { Records = Array.Empty<ArticleDto>() }
            };
        }

        var dateFilter = request.Memory.LastInteractionDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffZ");
        var endpoint = "services/data/v57.0/support/knowledgeArticles?pageSize=100";
        var sfRequest = new SalesforceRequest(endpoint, Method.Get, Creds);
        sfRequest.AddLocaleHeader(locale);

        var publishedArticles = await Client.ExecuteWithErrorHandling<PublishedArticlesResponse>(sfRequest);
        IEnumerable<PublishedArticleDto> filtered = publishedArticles?.Articles?
        .Where(a => a.LastPublishedDate > request.Memory.LastInteractionDate)
        ?? Enumerable.Empty<PublishedArticleDto>();

        var idsForVisibility = filtered.Select(a => a.Id).ToArray();
        var visMap = await LoadVisibilityByArticleIdAsync(idsForVisibility, locale);

        filtered = filtered.Select(a =>
        {
            if (a is null) return a;
            if (visMap.TryGetValue(a.Id, out var v))
            {
                a.IsVisibleInPkb = v.IsVisibleInPkb;
                a.IsVisibleInCsp = v.IsVisibleInCsp;
            }
            return a;
        });

        if (visibility?.IsVisibleInPkb is bool wantPkb)
            filtered = filtered.Where(a => (a.IsVisibleInPkb ?? false) == wantPkb);

        if (visibility?.IsVisibleInCsp is bool wantCsp)
            filtered = filtered.Where(a => (a.IsVisibleInCsp ?? false) == wantCsp);

        if (!string.IsNullOrEmpty(category?.CategoryName))
        {
            filtered = filtered.Where(a =>
                a.CategoryGroups != null &&
                a.CategoryGroups.Any(cg =>
                    cg.SelectedCategories != null &&
                    cg.SelectedCategories.Any(sc => sc.CategoryName == category.CategoryName)));
        }
        if (!string.IsNullOrEmpty(category?.GroupName))
        {
            filtered = filtered.Where(a =>
                a.CategoryGroups != null &&
                a.CategoryGroups.Any(cg => cg.GroupName == category.GroupName));
        }

        if (category?.ExcludedDataCategories?.Any() == true)
        {
            var excludedSet = new HashSet<string>(category.ExcludedDataCategories, StringComparer.OrdinalIgnoreCase);

            filtered = filtered.Where(a =>
                a.CategoryGroups == null || !a.CategoryGroups.Any(cg =>
                    cg.SelectedCategories != null &&
                    cg.SelectedCategories.Any(sc =>
                        !string.IsNullOrEmpty(sc.CategoryName) &&
                        excludedSet.Contains(sc.CategoryName))));
        }

        if (category?.ExcludedGroupNames?.Any() == true)
        {
            var excludedGroups = new HashSet<string>(category.ExcludedGroupNames, StringComparer.OrdinalIgnoreCase);

            filtered = filtered.Where(a =>
                a.CategoryGroups == null ||
                !a.CategoryGroups.Any(cg =>
                    !string.IsNullOrEmpty(cg.GroupName) &&
                    excludedGroups.Contains(cg.GroupName)));
        }

        var records = filtered
         .Select(a => new ArticleDto(a, locale))
         .ToArray();

        return new PollingEventResponse<DateMemory, ListAllArticlesResponse>
        {
            FlyBird = records.Length > 0,
            Memory = new DateMemory { LastInteractionDate = DateTime.UtcNow },
            Result = new ListAllArticlesResponse { Records = records }
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

    private async Task<Dictionary<string, KnowledgeArticleVisibilityDto>> LoadVisibilityByArticleIdAsync(
    IEnumerable<string> articleIds, string locale)
    {
        var ids = articleIds.Distinct().ToArray();
        if (ids.Length == 0) return new();

        string q1 =
            $"SELECT KnowledgeArticleId, Id, IsVisibleInPkb, IsVisibleInCsp " +
            $"FROM KnowledgeArticleVersion " +
            $"WHERE PublishStatus = 'Online' AND Language = '{locale}' " +
            $"AND KnowledgeArticleId IN ({string.Join(",", ids.Select(x => $"'{x}'"))})";

        var req = new SalesforceRequest("services/data/v57.0/query", Method.Get, Creds);
        req.AddQueryParameter("q", q1);
        req.AddLocaleHeader(locale);

        var r1 = await Client.ExecuteWithErrorHandling<SoqlResponse<KnowledgeArticleVisibilityDto>>(req);
        var map = r1?.Records?.ToDictionary(x => x.KnowledgeArticleId, x => x)
                  ?? new Dictionary<string, KnowledgeArticleVisibilityDto>();

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

            var r2 = await Client.ExecuteWithErrorHandling<SoqlResponse<KnowledgeArticleVisibilityDto>>(req2);
            map = r2?.Records?
                .GroupBy(x => x.KnowledgeArticleId)
                .ToDictionary(g => g.Key, g => g.First())
                ?? new Dictionary<string, KnowledgeArticleVisibilityDto>();
        }

        return map;
    }
    private async Task<MasterArticleDto[]> GetArticles(string endpoint)
    {
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ListAllArticlesPollingResponse>(request);
        return response.Records.Where(x => x.IsDeleted == false).ToArray();
    }
}
