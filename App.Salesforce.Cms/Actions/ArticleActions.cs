using App.Salesforce.Cms.Dtos;
using App.Salesforce.Cms.Models.Requests;
using App.Salesforce.Cms.Models.Responses;
using Apps.Salesforce.Cms;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Actions
{
    [ActionList]
    public class ArticleActions
    {
        #region List actions

        [Action("List all master knowledge articles", Description = "List all master knowledge articles")]
        public ListAllArticlesResponse ListAllArticles(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var query = "SELECT FIELDS(ALL) FROM KnowledgeArticle LIMIT 200";
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/query?q={query}", Method.Get, authenticationCredentialsProviders);
            return client.Get<ListAllArticlesResponse>(request);
        }

        [Action("List all published articles translations", Description = "List all published articles translations")]
        public ListAllArticlesResponse ListPublishedArticlesTranslations(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] ListPublishedTranslationsRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var languageDetails = GetKnowledgeSettings(authenticationCredentialsProviders);

            var request = new SalesforceRequest($"services/data/v57.0/support/knowledgeArticles?pageSize=100", Method.Get, authenticationCredentialsProviders);
            request.AddLocaleHeader(input.Locale);

            var publishedArticles = client.Get<PublishedArticlesResponse>(request);
            return new ListAllArticlesResponse()
            {
                Records = publishedArticles.Articles.Select(a => new ArticleDto(a, languageDetails.DefaultLanguage))
            };
        }

        [Action("List all published articles", Description = "List all published articles")]
        public ListAllArticlesResponse ListAllPublishedArticles(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var languageDetails = GetKnowledgeSettings(authenticationCredentialsProviders);
            return ListPublishedArticlesTranslations(authenticationCredentialsProviders,
                new ListPublishedTranslationsRequest()
                {
                    Locale = languageDetails.DefaultLanguage
                });
        }

        [Action("List knowledge article versions", Description = "List knowledge article versions")]
        public ListAllArticlesVersionsResponse ListAllArticlesVersions(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleInfoRequest input)
        {
            var articleMetadata = GetArticleInfo(authenticationCredentialsProviders, input);
            var query = $"SELECT FIELDS(ALL) FROM {articleMetadata.ArticleType} WHERE KnowledgeArticleId = '{input.ArticleId}' LIMIT 200";
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/query?q={query}", Method.Get, authenticationCredentialsProviders);
            return client.Get<ListAllArticlesVersionsResponse>(request);
        }
        #endregion

        [Action("Get article info", Description = "Get article info by id")]
        public ArticleInfoDto GetArticleInfo(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleInfoRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/knowledgeManagement/articles/{input.ArticleId}", Method.Get, authenticationCredentialsProviders);
            return client.Get<ArticleInfoDto>(request);
        }

        [Action("Get all article content as object", Description = "Get all article content as object by id")]
        public ArticleContentDto GetArticleContent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleContentRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/support/knowledgeArticles/{input.ArticleId}", Method.Get, authenticationCredentialsProviders);
            request.AddLocaleHeader(input.Locale);
            return client.Get<ArticleContentDto>(request);
        }

        [Action("Get article custom content as object", Description = "Get article custom content only as object by id")]
        public GetArticleCustomContent GetArticleCustomContent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleContentRequest input)
        {
            return new GetArticleCustomContent()
            {
                Items = GetArticleContent(authenticationCredentialsProviders, input).LayoutItems.Where(i => i.Name.EndsWith("__c"))
            };
        }

        [Action("Get article content as HTML file", Description = "Get article content as HTML file by id")]
        public GetArticleContentAsHtmlResponse GetArticleContentAsHtml(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleContentRequest input)
        {
            var articleObject = GetArticleContent(authenticationCredentialsProviders, input);
            string customContent = "";
            foreach(var item in articleObject.LayoutItems)
            {
                if (item.Name.EndsWith("__c")) // custom field with content
                {
                    customContent += $"<h3>{item.Label}</h3>";
                    customContent += $"<div>{item.Value}</div><br>";
                }
            }
            string htmlFile = $"<html><head><title>{articleObject.Title}</title></head><body>{customContent}</body></html>";

            return new GetArticleContentAsHtmlResponse()
            {
                File = Encoding.ASCII.GetBytes(htmlFile),
                Filename = $"{articleObject.Title}.html"
            };
        }

        [Action("Get articles not translated in language", Description = "Get articles not translated in specific language")]
        public ListAllArticlesResponse GetArticlesNotTranslated(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] ListPublishedTranslationsRequest input)
        {
            var allArticles = ListAllPublishedArticles(authenticationCredentialsProviders).Records;
            var allTranslations = ListPublishedArticlesTranslations(authenticationCredentialsProviders, input).Records;
            return new ListAllArticlesResponse()
            {
                Records = allArticles.Where(a1 => !allTranslations.Any(a2 => a2.Id == a1.Id)).ToList()
            };
        }

        [Action("Submit knowledge article to translation", Description = "Submit knowledge article to translation")]
        public void SubmitToTranslation(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] SubmitToTranslationRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/actions/standard/submitKnowledgeArticleForTranslation", Method.Post, authenticationCredentialsProviders);
            request.AddJsonBody(new
            {
                inputs = new[]{
                    new {
                        articleId = input.ArticleId,
                        language = input.Locale,
                        assigneeId = input.AssigneeId 
                    } 
                }
            });
            client.Execute(request);
        }

        [Action("Publish knowledge article draft", Description = "Publish knowledge article draft")]
        public void PublishKnowledgeTranslation(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] PublishKnowledgeTranslationRequest input)
        {
            var versions = ListAllArticlesVersions(authenticationCredentialsProviders, new GetArticleInfoRequest() { ArticleId = input.ArticleId });
            var articleInDraft = versions.Records.Where(r => r.PublishStatus == "Draft" && r.Language == input.Locale).First();
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/actions/standard/publishKnowledgeArticles", Method.Post, authenticationCredentialsProviders);
            var pubAction = GetKnowledgeSettings(authenticationCredentialsProviders).DefaultLanguage == input.Locale ? 
                 "PUBLISH_ARTICLE" : "PUBLISH_TRANSLATION";
            request.AddJsonBody(new
            {
                inputs = new[]
                {
                    new
                    {
                        articleVersionIdList = new[]{ articleInDraft.Id },
                        pubAction = pubAction
                    } 
                }
            });
            client.Execute(request);
        }

        [Action("Create draft for knowledge article", Description = "Create draft for knowledge article")]
        public CreateArticleDraftResponse CreatedArticleDraft(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateArticleDraftRequest input)
        {
            var versions = ListAllArticlesVersions(authenticationCredentialsProviders, new GetArticleInfoRequest() { ArticleId = input.ArticleId });
            var articlePublished = versions.Records.Where(r => r.PublishStatus == "Online" && r.Language == input.Locale).First();
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/actions/standard/createDraftFromOnlineKnowledgeArticle", Method.Post, authenticationCredentialsProviders);
            var isTranslation = GetKnowledgeSettings(authenticationCredentialsProviders).DefaultLanguage != input.Locale;
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
            return new CreateArticleDraftResponse() 
            { 
                DraftVersionId = client.Execute<List<DraftResponseDto>>(request).Data.First().OutputValues.DraftId 
            };
        }

        [Action("Update knowledge article field", Description = "Update knowledge article field")]
        public void UpdateKnowledgeArticleField(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] UpdateKnowledgeArticleFieldRequest input)
        {
            var draftVersion = CreatedArticleDraft(authenticationCredentialsProviders, new CreateArticleDraftRequest() 
            { 
                ArticleId = input.ArticleId,
                Locale = input.Locale
            });
            var articleMetadata = GetArticleInfo(authenticationCredentialsProviders, new GetArticleInfoRequest() { ArticleId = input.ArticleId });
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v58.0/sobjects/{articleMetadata.ArticleType}/{draftVersion.DraftVersionId}", Method.Patch, authenticationCredentialsProviders);
            request.AddJsonBody(new Dictionary<string, string>()
            {
                { input.FieldName, input.FieldValue }
            });
            client.Execute(request);
            PublishKnowledgeTranslation(authenticationCredentialsProviders,
                new PublishKnowledgeTranslationRequest()
                {
                    ArticleId = input.ArticleId,
                    Locale = input.Locale
                });
        }

        [Action("Get knowledge language settings", Description = "Get knowledge language settings")]
        public KnowledgeSettingsDto GetKnowledgeSettings(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"/services/data/v57.0/knowledgeManagement/settings", Method.Get, authenticationCredentialsProviders);
            var settings = client.Get<KnowledgeSettingsDto>(request);
            return settings;
        }
    }
}
