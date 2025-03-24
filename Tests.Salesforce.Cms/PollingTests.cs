using Apps.Salesforce.Cms.Polling;
using Apps.Salesforce.Cms.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Salesforce.CmsTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Salesforce.Cms
{
    [TestClass]
    public class PollingTests:TestBase
    {
        [TestMethod]
        public async Task OnArticleCreated_IsSuccessful()
        {
            var polling = new ArticlePollingList(InvocationContext);

            var initialMemory = new DateMemory
            {
                LastInteractionDate = DateTime.Parse("2023-07-18T15:30:08.0000000Z")
            };

            var request = new PollingEventRequest<DateMemory>
            {
                Memory = initialMemory
            };

            var result = await polling.OnArticlesCreated(request);
            var articles = result.Result.Records;

            foreach (var article in articles)
            {
                Console.WriteLine($"ID: {article.Id}, CreatedDate: {article.CreatedDate}");
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task OnArticleUpdated_IsSuccessful()
        {
            var polling = new ArticlePollingList(InvocationContext);

            var initialMemory = new DateMemory
            {
                LastInteractionDate = DateTime.Parse("2023-07-18T15:30:08.0000000Z")
            };

            var request = new PollingEventRequest<DateMemory>
            {
                Memory = initialMemory
            };

            var result = await polling.OnArticlesUpdated(request);

            var articles = result.Result.Records;

            foreach (var article in articles)
            {
                Console.WriteLine($"ID: {article.Id}, UpdatedDate: {article.LastModifiedDate}");
            }

            Assert.IsNotNull(result);
        }
    }
}
