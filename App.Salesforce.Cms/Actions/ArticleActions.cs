using App.Salesforce.Cms.Models.Requests;
using App.Salesforce.Cms.Models.Responses;
using Apps.Salesforce.Cms;
using Apps.Salesforce.Cms.Api;
using Apps.Salesforce.Cms.Constants;
using Apps.Salesforce.Cms.Helper;
using Apps.Salesforce.Cms.Models.Dtos;
using Apps.Salesforce.Cms.Models.Identifiers;
using Apps.Salesforce.Cms.Models.Requests;
using Apps.Salesforce.Cms.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Mime;
using System.Text;

namespace App.Salesforce.Cms.Actions;

[ActionList("Articles")]
public class ArticleActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : SalesforceInvocable(invocationContext)
{
    #region List actions

    [Action("Search master knowledge articles", Description = "Search all master knowledge articles")]
    public async Task<ListAllMasterArticlesResponse> ListAllArticles([ActionParameter] MasterArticleSearchFilters input)
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
        [ActionParameter] LocaleIdentifier locale, 
        [ActionParameter] CategoryFilterRequest category)
    {
        var languageDetails = await GetKnowledgeSettings();
        var result = await FetchPublishedArticles(locale.Locale);

        if (!string.IsNullOrEmpty(category.CategoryName))
        {
            result.Articles = result.Articles.Where(x => x.CategoryGroups.Any(cg => cg.SelectedCategories.Any(sc => sc.CategoryName == category.CategoryName))).ToList();
        }
        if (!string.IsNullOrEmpty(category.GroupName))
        {
            result.Articles = result.Articles.Where(x => x.CategoryGroups.Any(cg => cg.GroupName == category.GroupName)).ToList();
        }

        if (category.ExcludedDataCategories?.Any() == true)
        {
            var excludedSet = new HashSet<string>(category.ExcludedDataCategories, StringComparer.OrdinalIgnoreCase);

            result.Articles = result.Articles
                .Where(x =>
                    x.CategoryGroups == null || !x.CategoryGroups.Any(cg =>
                        cg.SelectedCategories != null &&
                        cg.SelectedCategories.Any(sc =>
                            !string.IsNullOrEmpty(sc.CategoryName) &&
                            excludedSet.Contains(sc.CategoryName))))
                .ToList();
        }

        return new ListAllArticlesResponse
        {
            Records = result!.Articles
                .Select(a => new ArticleDto(a, languageDetails.DefaultLanguage))
                .ToList()
        };
    }

    [Action("Search published articles", Description = "Search all published articles")]
    public async Task<ListAllArticlesResponse> ListAllPublishedArticles([ActionParameter] SearchFilter input)
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
        if (input.ExcludedDataCategories?.Any() == true)
        {
            var excludedSet = new HashSet<string>(input.ExcludedDataCategories, StringComparer.OrdinalIgnoreCase);

            result.Records = result.Records
                .Where(x =>
                    x.CategoryGroups == null || !x.CategoryGroups.Any(cg =>
                        cg.SelectedCategories != null &&
                        cg.SelectedCategories.Any(sc =>
                            !string.IsNullOrEmpty(sc.CategoryName) &&
                            excludedSet.Contains(sc.CategoryName))))
                .ToList();
        }
        if (input?.ExcludedGroupNames?.Any() == true)
        {
            var excludedGroups = new HashSet<string>(input.ExcludedGroupNames, StringComparer.OrdinalIgnoreCase);

            result.Records = result.Records.Where(a =>
                a.CategoryGroups == null ||
                !a.CategoryGroups.Any(cg =>
                    !string.IsNullOrEmpty(cg.GroupName) &&
                    excludedGroups.Contains(cg.GroupName)));
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
        [ActionParameter] ArticleIdentifier input)
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
    public async Task<ArticleInfoDto> GetArticleInfo([ActionParameter] ArticleIdentifier input)
    {
        var endpoint = $"services/data/v57.0/knowledgeManagement/articles/{input.ArticleId}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return await Client.ExecuteWithErrorHandling<ArticleInfoDto>(request);
    }

    [Action("Get all article content as object", Description = "Get all article content as object by id")]
    public async Task<ArticleContentDto> GetArticleContent(
        [ActionParameter] ArticleIdentifier articleInput,
        [ActionParameter] LocaleIdentifier input)
    {
        var endpoint = $"services/data/v57.0/support/knowledgeArticles/{articleInput.ArticleId}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        request.AddLocaleHeader(input.Locale);

        return await Client.ExecuteWithErrorHandling<ArticleContentDto>(request);
    }

    [Action("Get article custom content as object", Description = "Get article custom content only as object by id")]
    public async Task<GetArticleCustomContent> GetArticleCustomContent(
        [ActionParameter] ArticleIdentifier articleInput,
        [ActionParameter] LocaleIdentifier input)
    {
        var content = await GetArticleContent(articleInput, input);

        return new()
        {
            Items = content.LayoutItems.Where(i => i.Name.EndsWith("__c"))
        };
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download article", Description = "Download article content as HTML file")]
    public async Task<DownloadArticleResponse> GetArticleContentAsHtml(
        [ActionParameter] DownloadArticleRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        var articleId = new ArticleIdentifier { ArticleId = input.ContentId };
        var article = await GetArticleContent(articleId, locale);

        var items = article.LayoutItems.ToList();
        if (input.FieldsToExclude != null && input.FieldsToExclude.Any())
            items.RemoveAll(i => input.FieldsToExclude.Contains(i.Name));

        var contentFields = new Dictionary<string, CustomContentFieldDto>();
        foreach (var item in items)
        {
            if (item.Name.EndsWith("__c"))  // Custom field with content
                contentFields[item.Name] = new CustomContentFieldDto(item.Label, item.Value);
        }

        var doc = HtmlHelper.GenerateHtml(contentFields);

        HtmlHelper.InjectHeadMetadata(doc, locale.Locale, MetadataConstants.Locale);
        HtmlHelper.InjectHeadMetadata(doc, input.ContentId, MetadataConstants.ArticleId);
        HtmlHelper.InjectTitle(doc, article.Title);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml));
        var file = await fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{article.Title}.html");
        return new(file);
    }

    [Action("Get article ID from a file", Description = "Get article ID from file metadata")]
    public async Task<string> GetArticleIdFromHtmlFile([ActionParameter] GetIdFromFileRequest input)
    {
        using var file = await fileManagementClient.DownloadAsync(input.File);
        string html = Encoding.UTF8.GetString(await file.GetByteData());

        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, input.File.Name).Target().Serialize()
                ?? throw new PluginMisconfigurationException("XLIFF did not contain files");
        }

        var doc = html.AsHtmlDocument();

        var articleId = HtmlHelper.ExtractHeadMetadata(doc, MetadataConstants.ArticleId);
        if (string.IsNullOrWhiteSpace(articleId))
        {
            throw new PluginApplicationException(
                "Couldn't find 'blackbird-article-id' metadata in the file. " +
                "Ensure the file was generated by the 'Download article' action."
            );
        }

        return articleId;
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload article", Description = "Upload article from a file")]
    public async Task TranslateFromHtml([ActionParameter] UploadArticleRequest input)
    {
        using var file = await fileManagementClient.DownloadAsync(input.Content);
        string html = Encoding.UTF8.GetString(await file.GetByteData());

        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, input.Content.Name).Target().Serialize() 
                ?? throw new PluginMisconfigurationException("XLIFF did not contain files");
        }

        var doc = html.AsHtmlDocument();

        string articleId = input.ContentId 
            ?? HtmlHelper.ExtractHeadMetadata(doc, MetadataConstants.ArticleId)
            ?? throw new PluginMisconfigurationException(
                "Article ID is required, it needs to be present either in the input or file"
            );

        var fieldsToUpdate = new Dictionary<string, string>();

        string? title = HtmlHelper.ExtractTitle(doc);
        if (!string.IsNullOrWhiteSpace(title))
            fieldsToUpdate["Title"] = title;

        var body = doc.DocumentNode.SelectSingleNode("//body");
        if (body != null)
        {
            foreach (var nodeField in body.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element))
            {
                var fieldName = nodeField.GetAttributeValue("data-fieldName", string.Empty);
                var divNode = nodeField.SelectSingleNode("div");
                if (divNode != null)
                {
                    var text = divNode.InnerHtml;
                    fieldsToUpdate.Add(fieldName, text);
                }
            }
        }

        await ErrorHandler.ExecuteWithErrorHandling(() =>
            UpdateMultipleArticleFields(articleId, input.Locale, fieldsToUpdate, input.Publish)
        );
    }

    [Action("Get articles not translated in language",
        Description = "Get articles not translated in specific language")]
    public async Task<ListAllArticlesResponse> GetArticlesNotTranslated([ActionParameter] LocaleIdentifier locale)
    {
        var allArticles = (await ListAllPublishedArticles(new SearchFilter())).Records;
        var allTranslations = (await ListPublishedArticlesTranslations(locale, new())).Records;

        return new()
        {
            Records = allArticles.Where(a1 => !allTranslations.Any(a2 => a2.Id == a1.Id)).ToList()
        };
    }

    [Action("Submit knowledge article to translation", Description = "Submit knowledge article to translation")]
    public async Task SubmitToTranslation(
        [ActionParameter] ArticleIdentifier articleInput,
        [ActionParameter] SubmitToTranslationRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        var endpoint = "services/data/v57.0/actions/standard/submitKnowledgeArticleForTranslation";
        var request = new SalesforceRequest(endpoint, Method.Post, Creds);
        request.AddJsonBody(new
        {
            inputs = new[]
            {
                new
                {
                    articleId = articleInput.ArticleId,
                    language = locale.Locale,
                    assigneeId = input.AssigneeId,
                    dueDate = input.DueDate,
                    sendEmailNotification = input.SendEmailNotification
                }
            }
        });

        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Publish knowledge article draft", Description = "Publish knowledge article draft")]
    public async Task PublishKnowledgeTranslation(
        [ActionParameter] ArticleIdentifier articleInput,
        [ActionParameter] PublishKnowledgeTranslationRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        var versions = await ListAllArticlesVersions(new() { ArticleId = articleInput.ArticleId });
        var articleInDraft = versions.Records
            .First(r => r.PublishStatus == "Draft" && r.Language == locale.Locale);

        var pubAction = (await GetKnowledgeSettings()).DefaultLanguage == locale.Locale
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
        [ActionParameter] ArticleIdentifier articleInput,
        [ActionParameter] CreateArticleDraftRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        var versions = await ListAllArticlesVersions(new() { ArticleId = articleInput.ArticleId })
        ?? throw new PluginApplicationException("No versions returned for the article.");

        var existingDraft = versions.Records
            .FirstOrDefault(x => x.Language == locale.Locale &&
                                 x.PublishStatus.Equals("Draft", StringComparison.OrdinalIgnoreCase));
        if (existingDraft != null)
            return new() { DraftVersionId = existingDraft.Id };

        var isTranslation = (await GetKnowledgeSettings()).DefaultLanguage != locale.Locale;

        var onlineSameLocale = versions.Records
            .FirstOrDefault(x => x.Language == locale.Locale &&
                                 x.PublishStatus.Equals("Online", StringComparison.OrdinalIgnoreCase));

        var endpoint = "services/data/v57.0/actions/standard/createDraftFromOnlineKnowledgeArticle";
        var request = new SalesforceRequest(endpoint, Method.Post, Creds);

        if (onlineSameLocale != null)
        {
            request.AddJsonBody(new
            {
                inputs = new[]
                {
                new
                {
                    action = isTranslation ? "EDIT_AS_DRAFT_TRANSLATION" : "EDIT_AS_DRAFT_ARTICLE",
                    unpublish = input.Unpublish ?? false,
                    articleVersionId = onlineSameLocale.Id
                }
            }
            });
        }
        else
        {
            var masterLang = (await GetKnowledgeSettings()).DefaultLanguage;
            var masterOnline = versions.Records
                .FirstOrDefault(x => x.Language == masterLang &&
                                     x.PublishStatus.Equals("Online", StringComparison.OrdinalIgnoreCase))
                ?? throw new PluginApplicationException(
                    $"No Online version found in master language '{masterLang}' to create translation draft from.");

            request.AddJsonBody(new
            {
                inputs = new[]
                {
                new
                {
                    action = "CREATE_TRANSLATION_DRAFT",
                    language = locale.Locale,
                    articleVersionId = masterOnline.Id,
                    unpublish = false
                }
            }
            });
        }

        var response = await Client.ExecuteWithErrorHandling(request);
        var draftData = JsonConvert.DeserializeObject<DraftResponseDto[]>(response.Content);
        var draftId = draftData?.FirstOrDefault()?.OutputValues?.DraftId;

        if (string.IsNullOrWhiteSpace(draftId))
        {
            var error = JsonConvert.DeserializeObject<DraftErrorDto[]>(response.Content);

            string? extractedMsg = null;
            var firstErr = error?.FirstOrDefault();
            if (firstErr?.OutputValues != null && firstErr.OutputValues.Any())
            {
                var kv = firstErr.OutputValues.First();
                extractedMsg = kv.Value;
            }

            var msg = extractedMsg ?? $"Draft creation failed. Raw: {response.Content}";
            throw new PluginApplicationException(msg);
        }

        return new() { DraftVersionId = draftId };
    }

    [Action("Update knowledge article field", Description = "Update knowledge article field")]
    public async Task UpdateKnowledgeArticleField(
        [ActionParameter] ArticleIdentifier articleInput,
        [ActionParameter] UpdateKnowledgeArticleFieldRequest input,
        [ActionParameter] LocaleIdentifier locale)
    {
        await UpdateMultipleArticleFields(
            articleInput.ArticleId,
            locale.Locale,
            new() { { input.FieldName, input.FieldValue } }, 
            input.Publish
        );
    }

    [Action("Get knowledge language settings", Description = "Get knowledge language settings")]
    public async Task<KnowledgeSettingsDto> GetKnowledgeSettings()
    {
        var endpoint = "/services/data/v57.0/knowledgeManagement/settings";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return await Client.ExecuteWithErrorHandling<KnowledgeSettingsDto>(request);
    }

    #region Utils

    [Action("DEBUG: Get auth data", Description = "Can be used only for debugging purposes.")]
    public List<AuthenticationCredentialsProvider> GetAuthenticationCredentialsProviders()
    {
        return InvocationContext.AuthenticationCredentialsProviders.ToList();
    }

    private async Task UpdateMultipleArticleFields(string articleId, string locale, Dictionary<string, string> fields, bool publishChanges)
    {
        var articleIdentifier = new ArticleIdentifier { ArticleId = articleId };
        var draftVersion = await CreatedArticleDraft(
            articleIdentifier,
            new CreateArticleDraftRequest { },
            new LocaleIdentifier { Locale = locale }
        );

        var articleMetadata = await GetArticleInfo(articleIdentifier);

        var endpoint = $"services/data/v58.0/sobjects/{articleMetadata.ArticleType}/{draftVersion.DraftVersionId}";
        var request = new SalesforceRequest(endpoint, Method.Patch, Creds);
        request.AddJsonBody(fields);

        await Client.ExecuteWithErrorHandling(request);
        if (publishChanges)
        {
            await PublishKnowledgeTranslation(
                articleIdentifier, 
                new PublishKnowledgeTranslationRequest(),
                new LocaleIdentifier { Locale = locale }
            );
        }
    }

    #endregion
}