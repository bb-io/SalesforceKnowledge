using System.Net.Mime;
using App.Salesforce.Cms.Models.Requests;
using App.Salesforce.Cms.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using System.Text;
using App.Salesforce.Cms.Actions.Base;
using App.Salesforce.Cms.Api;
using App.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Newtonsoft.Json;
using RestSharp;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Common.Authentication;
using System.Net;
using Apps.Salesforce.Cms.Models.Requests;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Apps.Salesforce.Cms.Models.Dtos;

namespace App.Salesforce.Cms.Actions;

[ActionList]
public class ArticleActions : SalesforceActions
{
    private readonly IFileManagementClient _fileManagementClient;
    public ArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    #region List actions

    [Action("Search master knowledge articles", Description = "Search all master knowledge articles")]
    public async Task<ListAllMasterArticlesResponse> ListAllArticles([ActionParameter] masterArticleSearchFilters input)
    {
        var query = "SELECT FIELDS(ALL) FROM KnowledgeArticle LIMIT 200";
        var endpoint = $"services/data/v57.0/query?q={query}";

        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        var result = await Client.ExecuteWithErrorHandling<ListAllMasterArticlesResponse>(request)!;
        result.Records = result.Records.Where(a => a.IsDeleted != true);

        #region filters
        if (input.CreatedBefore.HasValue)
        {
            result.Records = result.Records.Where(a => a.CreatedDate < input.CreatedBefore.Value);
        }
        if (input.CreatedAfter.HasValue)
        {
            result.Records = result.Records.Where(a => a.CreatedDate > input.CreatedAfter.Value);
        }
        if (input.UpdatedBefore.HasValue)
        {
            result.Records = result.Records.Where(a => a.LastModifiedDate < input.UpdatedBefore.Value);
        }
        if (input.UpdatedAfter.HasValue)
        {
            result.Records = result.Records.Where(a => a.LastModifiedDate > input.UpdatedAfter.Value);
        }
        if (input.PublishedBefore.HasValue)
        {
            result.Records = result.Records.Where(a => a.LastPublishedDate < input.PublishedBefore.Value);
        }
        if (input.PublishedAfter.HasValue)
        {
            result.Records = result.Records.Where(a => a.LastPublishedDate > input.PublishedAfter.Value);
        }
        if (input.Published.HasValue)
        {
            result.Records = result.Records.Where(a => input.Published.Value == a.LastPublishedDate.HasValue);
        }
        if (input.Archived.HasValue)
        {
            result.Records = result.Records.Where(a => input.Archived.Value == a.ArchivedDate.HasValue);
        }
        #endregion

        return result;
    }

    [Action("Search all published articles translations", Description = "Search all published articles translations")]
    public async Task<ListAllArticlesResponse> ListPublishedArticlesTranslations(
        [ActionParameter] ListPublishedTranslationsRequest input, [ActionParameter] CategoryFilterRequest category)
    {
        var languageDetails = await GetKnowledgeSettings();
        var result = await FetchPublishedArticles(input.Locale);

        if (!string.IsNullOrEmpty(category.CategoryName))
        {
            result.Articles = result.Articles.Where(x => x.CategoryGroups.Any(cg => cg.SelectedCategories.Any(sc => sc.CategoryName == category.CategoryName))).ToList();
        }
        if (!string.IsNullOrEmpty(category.GroupName))
        {
            result.Articles = result.Articles.Where(x => x.CategoryGroups.Any(cg => cg.GroupName == category.GroupName)).ToList();
        }

        return new ListAllArticlesResponse
        {
            Records = result!.Articles
                .Select(a => new ArticleDto(a, languageDetails.DefaultLanguage))
                .ToList()
        };
    }

    [Action("Search published articles", Description = "Search all published articles")]
    public async Task<ListAllArticlesResponse> ListAllPublishedArticles([ActionParameter]searchFilter input)
    {
        var languageDetails = await GetKnowledgeSettings();

        var publishedArticles = await FetchPublishedArticles(languageDetails.DefaultLanguage);

        var result = new ListAllArticlesResponse
        {
            Records = publishedArticles!.Articles
                .Select(a => new ArticleDto(a, languageDetails.DefaultLanguage))
                .ToList()
        };

        if (input.PublishedAfter.HasValue)
        {
            result.Records = result.Records.Where(x => x.LastPublishedDate > input.PublishedAfter.Value);
        }
        if (input.PublishedBefore.HasValue)
        {
            result.Records = result.Records.Where(x => x.LastPublishedDate > input.PublishedBefore.Value);
        }
        if (!string.IsNullOrEmpty(input.CategoryName))
        {
            result.Records = result.Records.Where(x => x.CategoryGroups.Any(cg => cg.SelectedCategories.Any(sc => sc.CategoryName == input.CategoryName)));
        }
        if (!string.IsNullOrEmpty(input.GroupName))
        {
            result.Records = result.Records.Where(x => x.CategoryGroups.Any(cg => cg.GroupName == input.GroupName));
        }

        return result;
    }

    private async Task<PublishedArticlesResponse> FetchPublishedArticles(string locale)
    {
        var endpoint = "services/data/v57.0/support/knowledgeArticles?pageSize=100";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        request.AddLocaleHeader(locale);
        return await Client.ExecuteWithErrorHandling<PublishedArticlesResponse>(request);
    }


    [Action("Search knowledge article versions", Description = "Search knowledge article versions")]
    public async Task<ListAllArticlesVersionsResponse?> ListAllArticlesVersions(
        [ActionParameter] ArticleRequest input)
    {
        var articleMetadata = await GetArticleInfo(input);
        var query =
            $"SELECT FIELDS(ALL) FROM {articleMetadata.ArticleType} WHERE KnowledgeArticleId = '{input.ArticleId}' LIMIT 200";

        var endpoint = $"services/data/v57.0/query?q={query}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return await Client.ExecuteWithErrorHandling<ListAllArticlesVersionsResponse>(request);
    }

    #endregion

    [Action("Get article info", Description = "Get article info by id")]
    public Task<ArticleInfoDto> GetArticleInfo([ActionParameter] ArticleRequest input)
    {
        var endpoint = $"services/data/v57.0/knowledgeManagement/articles/{input.ArticleId}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<ArticleInfoDto>(request)!;
    }

    [Action("Get all article content as object", Description = "Get all article content as object by id")]
    public Task<ArticleContentDto> GetArticleContent([ActionParameter] GetArticleContentRequest input)
    {
        var endpoint = $"services/data/v57.0/support/knowledgeArticles/{input.ArticleId}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        request.AddLocaleHeader(input.Locale);
        var response = Client.Execute(request);
        return Client.ExecuteWithErrorHandling<ArticleContentDto>(request)!;
    }

    [Action("Get article custom content as object", Description = "Get article custom content only as object by id")]
    public async Task<GetArticleCustomContent> GetArticleCustomContent(
        [ActionParameter] GetArticleContentRequest input)
    {
        var content = await GetArticleContent(input);

        return new()
        {
            Items = content.LayoutItems.Where(i => i.Name.EndsWith("__c"))
        };
    }

    [Action("Get article content as HTML file", Description = "Get article content as HTML file by id")]
    public async Task<GetArticleContentAsHtmlResponse> GetArticleContentAsHtml(
        [ActionParameter] GetArticleContentRequest input, [ActionParameter] ExcludeFieldsRequest itemsToExclude)
    {
        var articleObject = await GetArticleContent(input);

        var customContent = string.Empty;
        var items = articleObject.LayoutItems.ToList();
        if (itemsToExclude != null && itemsToExclude.Fields != null)
        {
            items.RemoveAll(i => itemsToExclude.Fields.Contains(i.Name));
        }
        foreach (var item in items)
        {
            if (item.Name.EndsWith("__c")) // custom field with content
            {
                customContent += $"<div data-fieldName=\"{item.Name}\"><h3>{item.Label}</h3>";
                customContent += $"<div>{item.Value}</div></div>";
            }
        }

        var metaTags = $@"<meta name=""blackbird-article-id"" content=""{input.ArticleId}"" /><meta name=""blackbird-locale"" content=""{input.Locale}"" /><meta charset=""UTF-8"">";

        var htmlFile = $@"
<!DOCTYPE html>
<html>
<head>
    <title>{articleObject.Title}</title>
    {metaTags}
</head>
<body>
    {customContent}
</body>
</html>";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlFile));
        var file = await _fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{articleObject.Title}.html");
        return new() { File = file };
    }

    [Action("Translate knowledge article from HTML file", Description = "Translate knowledge article from HTML file")]
    public Task TranslateFromHtml([ActionParameter] TranslateFromHtmlRequest input)
    {
        var fileBytes = _fileManagementClient.DownloadAsync(input.File).Result.GetByteData().Result;
        var doc = Encoding.UTF8.GetString(fileBytes).AsHtmlDocument();
        var body = doc.DocumentNode.SelectSingleNode("/html/body");

        var articleId = string.IsNullOrEmpty(input.ArticleId)
        ? doc.DocumentNode
              .SelectSingleNode("//meta[@name='blackbird-article-id']")
              ?.GetAttributeValue("content", null)
        : input.ArticleId;

        if (string.IsNullOrEmpty(articleId))
            throw new PluginMisconfigurationException("Article ID is required, it needs to be present either in the input or HTML file.");

        var fieldsToUpdate = new Dictionary<string, string>();
        foreach (var nodeField in body.ChildNodes.Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Element))
        {
            var fileName = nodeField.GetAttributeValue("data-fieldName", string.Empty);
            var divNode = nodeField.SelectSingleNode("div");
            if (divNode != null)
            {
                var text = divNode.InnerHtml;
                fieldsToUpdate.Add(fileName, text);
            }
        }

        return UpdateMultipleArticleFields(articleId, input.Locale, fieldsToUpdate);
    }

    [Action("Get articles not translated in language",
        Description = "Get articles not translated in specific language")]
    public async Task<ListAllArticlesResponse> GetArticlesNotTranslated(
        [ActionParameter] ListPublishedTranslationsRequest input)
    {
        var allArticles = (await ListAllPublishedArticles(new searchFilter())).Records;
        var allTranslations = (await ListPublishedArticlesTranslations(input, new())).Records;

        return new()
        {
            Records = allArticles.Where(a1 => !allTranslations.Any(a2 => a2.Id == a1.Id)).ToList()
        };
    }


    [Action("Submit knowledge article to translation", Description = "Submit knowledge article to translation")]
    public async Task SubmitToTranslation([ActionParameter] SubmitToTranslationRequest input)
    {
        var endpoint = "services/data/v57.0/actions/standard/submitKnowledgeArticleForTranslation";
        var request = new SalesforceRequest(endpoint, Method.Post, Creds);
        request.AddJsonBody(new
        {
            inputs = new[]
            {
                new
                {
                    articleId = input.ArticleId,
                    language = input.Locale,
                    assigneeId = input.AssigneeId,
                    dueDate = input.DueDate,
                    sendEmailNotification = input.SendEmailNotification
                }
            }
        });

        await Client.ExecuteWithErrorHandling(request);
    }


    [Action("Publish knowledge article draft", Description = "Publish knowledge article draft")]
    public async Task PublishKnowledgeTranslation([ActionParameter] PublishKnowledgeTranslationRequest input)
    {
        var versions = await ListAllArticlesVersions(new() { ArticleId = input.ArticleId });
        var articleInDraft = versions.Records
            .First(r => r.PublishStatus == "Draft" && r.Language == input.Locale);

        var pubAction = (await GetKnowledgeSettings()).DefaultLanguage == input.Locale
            ? "PUBLISH_ARTICLE"
            : "PUBLISH_TRANSLATION";

        var endpoint = "services/data/v57.0/actions/standard/publishKnowledgeArticles";
        var request = new SalesforceRequest(endpoint, Method.Post, Creds);
        request.AddJsonBody(new
        {
            inputs = new[]
            {
                new
                {
                    pubAction,
                    articleVersionIdList = new[] { articleInDraft.Id },
                    pubDate = input.PubDate?.ToString("yyyy-MM-ddTHH:mm:sszzz")
                }
            }
        });

        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Create draft for knowledge article", Description = "Create draft for knowledge article")]
    public async Task<CreateArticleDraftResponse> CreatedArticleDraft(
        [ActionParameter] CreateArticleDraftRequest input)
    {
        await PublishTranslationArticle(input.ArticleId, input.Locale);

        var versions = await ListAllArticlesVersions(new()
        {
            ArticleId = input.ArticleId
        });

        var articlePublished = versions.Records
            .First(r => r.PublishStatus == "Online" && r.Language == input.Locale);

        var isTranslation = (await GetKnowledgeSettings()).DefaultLanguage != input.Locale;

        var endpoint = "services/data/v57.0/actions/standard/createDraftFromOnlineKnowledgeArticle";
        var request = new SalesforceRequest(endpoint, Method.Post, Creds);
        request.AddJsonBody(new
        {
            inputs = new[]
            {
                new
                {
                    action = isTranslation ? "EDIT_AS_DRAFT_TRANSLATION" : "EDIT_AS_DRAFT_ARTICLE",
                    unpublish = input.Unpublish ?? false,
                    articleVersionId = articlePublished.Id
                }
            }
        });

        var response = await Client.ExecuteWithErrorHandling(request);

        var draftData = JsonConvert.DeserializeObject<DraftResponseDto[]>(response.Content);
        if (draftData?.FirstOrDefault()?.OutputValues.DraftId is null)
        {
            var error = JsonConvert.DeserializeObject<DraftErrorDto[]>(response.Content);
            throw new(error.First().OutputValues.First().Value);
        }

        return new()
        {
            DraftVersionId = draftData.First().OutputValues.DraftId
        };
    }

    [Action("Update knowledge article field", Description = "Update knowledge article field")]
    public Task UpdateKnowledgeArticleField([ActionParameter] UpdateKnowledgeArticleFieldRequest input)
    {
        return UpdateMultipleArticleFields(
            input.ArticleId,
            input.Locale,
            new()
            {
                { input.FieldName, input.FieldValue }
            });
    }


    [Action("Get knowledge language settings", Description = "Get knowledge language settings")]
    public Task<KnowledgeSettingsDto> GetKnowledgeSettings()
    {
        var endpoint = "/services/data/v57.0/knowledgeManagement/settings";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<KnowledgeSettingsDto>(request)!;
    }

    #region Utils

    [Action("DEBUG: Get auth data", Description = "Can be used only for debugging purposes.")]
    public List<AuthenticationCredentialsProvider> GetAuthenticationCredentialsProviders()
    {
        return InvocationContext.AuthenticationCredentialsProviders.ToList();
    }

    private async Task UpdateMultipleArticleFields(string articleId, string locale, Dictionary<string, string> fields)
    {
        var draftVersion = await CreatedArticleDraft(new()
        {
            ArticleId = articleId,
            Locale = locale
        });

        var articleMetadata = await GetArticleInfo(new()
        {
            ArticleId = articleId
        });

        var endpoint = $"services/data/v58.0/sobjects/{articleMetadata.ArticleType}/{draftVersion.DraftVersionId}";
        var request = new SalesforceRequest(endpoint, Method.Patch, Creds);
        request.AddJsonBody(fields);

        await Client.ExecuteWithErrorHandling(request);
        await PublishKnowledgeTranslation(new()
        {
            ArticleId = articleId,
            Locale = locale
        });
    }
    
    private async Task PublishTranslationArticle(string articleId, string locale)
    {
        await SubmitToTranslation(new()
        {
            ArticleId = articleId,
            Locale = locale
        });

        await PublishKnowledgeTranslation(new()
        {
            ArticleId = articleId,
            Locale = locale
        });
    }

    #endregion
}