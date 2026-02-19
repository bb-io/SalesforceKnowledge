using Salesforce.CmsTests.Base;
using Apps.Salesforce.Cms.Polling;
using Apps.Salesforce.Cms.Polling.Models;
using Apps.Salesforce.Cms.Models.Requests;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Tests.Salesforce.Cms;

[TestClass]
public class PollingTests : TestBase
{
    [TestMethod]
    public async Task OnArticlesCreatedOrUpdated_IsSuccessful()
    {
        // Arrange
        var polling = new ArticlePollingList(InvocationContext);
        var memory = new DateMemory { LastInteractionDate = new DateTime(2026, 02, 19, 16, 0, 0, DateTimeKind.Utc) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };

        // Act
        var result = await polling.OnArticlesUpdated(request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task OnArticlesCreated_IsSuccessful()
    {
        // Arrange
        var polling = new ArticlePollingList(InvocationContext);
        var memory = new DateMemory { LastInteractionDate = new DateTime(2026, 02, 19, 16, 0, 0, DateTimeKind.Utc) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };

        // Act
        var result = await polling.OnArticlesCreated(request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task OnPublishedArticlesCreated_IsSuccessful()
    {
        // Arrange
        var polling = new ArticlePollingList(InvocationContext);
        var memory = new DateMemory { LastInteractionDate = new DateTime(2026, 02, 19, 10, 0, 0, DateTimeKind.Utc) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };
        var category = new CategoryFilterRequest 
        { 
            GroupName = "Blackbird", 
            CategoryName = "All" 
        };
        var visibility = new VisibilityFilterRequest { IsVisibleInCsp = false };

        // Act
        var result = await polling.OnPublishedArticlesCreated(request, category, visibility);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}
