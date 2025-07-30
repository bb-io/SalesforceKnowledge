using App.Salesforce.Cms.Actions;
using App.Salesforce.Cms.Models.Requests;
using Apps.Salesforce.Cms.Models.Requests;
using Blackbird.Applications.Sdk.Common.Files;
using Salesforce.CmsTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Salesforce.Cms
{
    [TestClass]
    public class ArticleTests : TestBase
    {
        [TestMethod]
        public async Task SearchAllMasterKnowledge_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);

            var result = await action.ListAllArticles();

            foreach (var article in result.Records)
            {
                Console.WriteLine($"{article.Id} - {article.Title}");
                Assert.IsNotNull(article);
            }
        }

        [TestMethod]
        public async Task SearchAllPublishedTranslation_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new ListPublishedTranslationsRequest
            {
                Locale = "en_US"
            };
            var result = await action.ListPublishedArticlesTranslations(input);

            foreach (var article in result.Records)
            {
                Console.WriteLine($"{article.Id} - {article.Title}");
                Assert.IsNotNull(article);
            }
        }

        [TestMethod]
        public async Task SearchAllPublishedArticles_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var result = await action.ListAllPublishedArticles(new searchFilter());

            foreach (var article in result.Records)
            {
                Console.WriteLine($"{article.Id} - {article.Title}");
                Assert.IsNotNull(article);
            }
        }

        [TestMethod]
        public async Task SearchKnowledgeArticleVersions_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new ArticleRequest
            {
                ArticleId= "kA067000000E03gCAC"
            };
            var result = await action.ListAllArticlesVersions(input);

            foreach (var article in result.Records)
            {
                Console.WriteLine($"{article.Id} - {article.Title} - {article.IsLatestVersion}");
                Assert.IsNotNull(article);
            }
        }

        [TestMethod]
        public async Task GetArticleInfo_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new ArticleRequest
            {
                ArticleId = "kA067000000E03gCAC"
            };
            var result = await action.GetArticleInfo(input);

                Console.WriteLine($"{result.ArticleId} - {result.MasterLanguage}");
                Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetArticleContentAsObject_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new GetArticleContentRequest
            {
                ArticleId = "kA067000000E03gCAC",
                Locale = "en_US"
            };
            var result = await action.GetArticleContent(input);

 
                Console.WriteLine($"{result.Id} - {result.Title}");
                Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetArticleCustomContentAsObject_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new GetArticleContentRequest
            {
                ArticleId = "kA067000000E03gCAC",
                Locale = "en_US"
            };
            var result = await action.GetArticleCustomContent(input);

            foreach (var article in result.Items)
            {
                Console.WriteLine($"{article.Name} - {article.Type}");
                Assert.IsNotNull(result);
            }
           
        }

        [TestMethod]
        public async Task GetArticleContentAsHtml_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new GetArticleContentRequest
            {
                ArticleId = "kA067000000E03gCAC",
                Locale = "en_US"
            };
            var result = await action.GetArticleContentAsHtml(input);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetArticlesNotTranslated_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new ListPublishedTranslationsRequest
            {
                Locale = "en_US"
            };
            var result = await action.GetArticlesNotTranslated(input);

            foreach (var article in result.Records)
            {
                Console.WriteLine($"{article.Title} - {article.Id}");
                Assert.IsNotNull(result);
            }

        }

        [TestMethod]
        public async Task TranslateFromHtml_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new TranslateFromHtmlRequest
            {
                ArticleId = "kA067000000E03gCAC",
                Locale = "en_US",
                File = new FileReference
                {
                    Name = "test.html"
                }
            };
            var result = action.TranslateFromHtml(input);

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task SubmitKnowledgeArticleToTranslation_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new SubmitToTranslationRequest
            {
                ArticleId = "kA067000000E03gCAC",
                Locale = "en_US",
                AssigneeId = "0056700000Dd6t7AAB",
            };

            var result =  action.SubmitToTranslation(input);

            Assert.IsTrue(true);       
        }

        [TestMethod]
        public async Task PublishKnowledgeTranslation_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new PublishKnowledgeTranslationRequest
            {
                ArticleId = "ka0J5000000fy2ZIAQ",
                Locale = "en_US",
            };

            var result = action.PublishKnowledgeTranslation(input);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task CreateDraftKnowledgeTranslation_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new CreateArticleDraftRequest
            {
                ArticleId = "ka067000000EMXVAA4",
                Locale = "en_US",
            };

            var result = action.CreatedArticleDraft(input);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetKnowledgeSettings_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var result = await action.GetKnowledgeSettings();

            foreach (var article in result.Languages)
            {
                Console.WriteLine($"{article.Name} - {article.Active}");
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public async Task UpdateKnowledgeArticleField_IsSuccess()
        {
            var action = new ArticleActions(InvocationContext, FileManager);
            var input = new UpdateKnowledgeArticleFieldRequest
            {
                ArticleId = "kA067000000E03gCAC",
                FieldName = "title",
                FieldValue = "New Title 03/21",
                Locale = "en_US"
            };
            var result = action.UpdateKnowledgeArticleField(input);
            Assert.IsNotNull(true);
        }
    }
}
