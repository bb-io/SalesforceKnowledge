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
        [Action("List all articles", Description = "List all articles")]
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

        [Action("Get article info", Description = "Get article info by id")]
        public ArticleInfoDto GetArticleInfo(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleInfoRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/knowledgeManagement/articles/{input.ArticleId}", Method.Get, authenticationCredentialsProviders);
            return client.Get<ArticleInfoDto>(request);
        }

        [Action("Get article content as object", Description = "Get article content as object by id")]
        public ArticleContentDto GetArticleContent(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetArticleContentRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/support/knowledgeArticles/{input.ArticleId}", Method.Get, authenticationCredentialsProviders);
            request.AddLocaleHeader(input.Locale);
            return client.Get<ArticleContentDto>(request);
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

        [Action("Get knowledge language settings", Description = "Get knowledge language settings")]
        public KnowledgeSettingsDto GetKnowledgeSettings(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"/services/data/v57.0/knowledgeManagement/settings", Method.Get, authenticationCredentialsProviders);
            var settings = client.Get<KnowledgeSettingsDto>(request);
            settings.DefaultLanguage = settings.DefaultLanguage.ToLower().Replace("_", "-");
            settings.Languages.ForEach(l => { l.Name = l.Name.ToLower().Replace("_", "-"); });
            return settings;
        }
    }
}
