using Apps.Salesforce.Cms.Polling;
using Apps.Salesforce.Cms.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Salesforce.CmsTests.Base;

namespace Tests.Salesforce.Cms;

[TestClass]
public class PollingTests:TestBase
{
    [TestMethod]
    public async Task OnArticleCreated_IsSuccessful()
    {
        var polling = new ArticlePollingList(InvocationContext);

        var initialMemory = new DateMemory
        {
            LastInteractionDate = DateTime.Parse("2024-08-04T15:30:08.0000000Z")
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
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task OnPublishedArticleCreated_IsSuccessful()
    {
        var polling = new ArticlePollingList(InvocationContext);

        var initialMemory = new DateMemory
        {
            LastInteractionDate = DateTime.Parse("2025-08-06T15:30:08.0000000Z")
        };

        var request = new PollingEventRequest<DateMemory>
        {
            Memory = initialMemory
        };

        var result = await polling.OnPublishedArticlesCreated(request);
        var articles = result.Result.Records;

        foreach (var article in articles)
        {
            Console.WriteLine($"ID: {article.Id}, LastPublishedDate: {article.LastPublishedDate}");
        }
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);

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
