using System.Net.Mime;
using App.Salesforce.Cms.Dtos;
using App.Salesforce.Cms.Models.Requests;
using App.Salesforce.Cms.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using System.Text;
using App.Salesforce.Cms.Actions.Base;
using App.Salesforce.Cms.Api;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using RestSharp;

namespace App.Salesforce.Cms.Actions;

[ActionList]
public class ArticleActions : SalesforceActions
{
    public ArticleActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    #region List actions

    [Action("List all master knowledge articles", Description = "List all master knowledge articles")]
    public Task<ListAllArticlesResponse> ListAllArticles()
    {
        var query = "SELECT FIELDS(ALL) FROM KnowledgeArticle LIMIT 200";
        var endpoint = $"services/data/v57.0/query?q={query}";

        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        return Client.GetAsync<ListAllArticlesResponse>(request)!;
    }

    [Action("List all published articles translations", Description = "List all published articles translations")]
    public async Task<ListAllArticlesResponse> ListPublishedArticlesTranslations(
        [ActionParameter] ListPublishedTranslationsRequest input)
    {
        var languageDetails = await GetKnowledgeSettings();

        var endpoint = "services/data/v57.0/support/knowledgeArticles?pageSize=100";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        request.AddLocaleHeader(input.Locale);

        var publishedArticles = await Client.GetAsync<PublishedArticlesResponse>(request);
        return new()
        {
            Records = publishedArticles!.Articles
                .Select(a => new ArticleDto(a, languageDetails.DefaultLanguage))
        };
    }

    [Action("List all published articles", Description = "List all published articles")]
    public async Task<ListAllArticlesResponse> ListAllPublishedArticles()
    {
        var languageDetails = await GetKnowledgeSettings();

        return await ListPublishedArticlesTranslations(
            new()
            {
                Locale = languageDetails.DefaultLanguage
            });
    }

    [Action("List knowledge article versions", Description = "List knowledge article versions")]
    public async Task<ListAllArticlesVersionsResponse?> ListAllArticlesVersions(
        [ActionParameter] ArticleRequest input)
    {
        var articleMetadata = await GetArticleInfo(input);
        var query =
            $"SELECT FIELDS(ALL) FROM {articleMetadata.ArticleType} WHERE KnowledgeArticleId = '{input.ArticleId}' LIMIT 200";

        var endpoint = $"services/data/v57.0/query?q={query}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return await Client.GetAsync<ListAllArticlesVersionsResponse>(request);
    }

    #endregion

    [Action("Get article info", Description = "Get article info by id")]
    public Task<ArticleInfoDto> GetArticleInfo([ActionParameter] ArticleRequest input)
    {
        var endpoint = $"services/data/v57.0/knowledgeManagement/articles/{input.ArticleId}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);

        return Client.GetAsync<ArticleInfoDto>(request)!;
    }

    [Action("Get all article content as object", Description = "Get all article content as object by id")]
    public Task<ArticleContentDto> GetArticleContent([ActionParameter] GetArticleContentRequest input)
    {
        var endpoint = $"services/data/v57.0/support/knowledgeArticles/{input.ArticleId}";
        var request = new SalesforceRequest(endpoint, Method.Get, Creds);
        request.AddLocaleHeader(input.Locale);

        return Client.GetAsync<ArticleContentDto>(request)!;
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
        [ActionParameter] GetArticleContentRequest input)
    {
        var articleObject = await GetArticleContent(input);

        var customContent = string.Empty;
        foreach (var item in articleObject.LayoutItems)
        {
            if (item.Name.EndsWith("__c")) // custom field with content
            {
                customContent += $"<div data-fieldName=\"{item.Name}\"><h3>{item.Label}</h3>";
                customContent += $"<div>{item.Value}</div></div>";
            }
        }

        var htmlFile = (articleObject.Title, customContent).AsHtml();

        return new()
        {
            File = new(Encoding.ASCII.GetBytes(htmlFile))
            {
                Name = $"{articleObject.Title}.html",
                ContentType = MediaTypeNames.Text.Html
            },
        };
    }

    [Action("Translate knowledge article from HTML file", Description = "Translate knowledge article from HTML file")]
    public Task TranslateFromHtml([ActionParameter] TranslateFromHtmlRequest input)
    {
        var doc = Encoding.ASCII.GetString(input.File.Bytes).AsHtmlDocument();
        var body = doc.DocumentNode.SelectSingleNode("/html/body");

        var fieldsToUpdate = new Dictionary<string, string>();
        foreach (var nodeField in body.ChildNodes)
        {
            var fileName = nodeField.GetAttributeValue<string>("data-fieldName", string.Empty);
            var text = nodeField.SelectSingleNode("div").InnerHtml;
            fieldsToUpdate.Add(fileName, text);
        }

        return UpdateMultipleArticleFields(input.ArticleId, input.Locale, fieldsToUpdate);
    }

    [Action("Get articles not translated in language",
        Description = "Get articles not translated in specific language")]
    public async Task<ListAllArticlesResponse> GetArticlesNotTranslated(
        [ActionParameter] ListPublishedTranslationsRequest input)
    {
        var allArticles = (await ListAllPublishedArticles()).Records;
        var allTranslations = (await ListPublishedArticlesTranslations(input)).Records;

        return new()
        {
            Records = allArticles.Where(a1 => !allTranslations.Any(a2 => a2.Id == a1.Id)).ToList()
        };
    }

    [Action("Submit knowledge article to translation", Description = "Submit knowledge article to translation")]
    public Task SubmitToTranslation([ActionParameter] SubmitToTranslationRequest input)
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
                    assigneeId = input.AssigneeId
                }
            }
        });

        return Client.ExecuteAsync(request);
    }

    [Action("Publish knowledge article draft", Description = "Publish knowledge article draft")]
    public async Task PublishKnowledgeTranslation([ActionParameter] PublishKnowledgeTranslationRequest input)
    {
        var versions = await ListAllArticlesVersions(new() { ArticleId = input.ArticleId });
        var articleInDraft = versions.Records.Where(r => r.PublishStatus == "Draft" && r.Language == input.Locale)
            .First();

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
                    articleVersionIdList = new[] { articleInDraft.Id }, pubAction
                }
            }
        });

        await Client.ExecuteAsync(request);
    }

    [Action("Create draft for knowledge article", Description = "Create draft for knowledge article")]
    public async Task<CreateArticleDraftResponse> CreatedArticleDraft(
        [ActionParameter] CreateArticleDraftRequest input)
    {
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
                    unpublish = false,
                    articleVersionId = articlePublished.Id
                }
            }
        });

        var response = await Client.ExecuteAsync<List<DraftResponseDto>>(request);
        return new()
        {
            DraftVersionId = response.Data!.First().OutputValues.DraftId
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

        return Client.GetAsync<KnowledgeSettingsDto>(request)!;
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

        await Client.ExecuteAsync(request);
        await PublishKnowledgeTranslation(new()
        {
            ArticleId = articleId,
            Locale = locale
        });
    }
}