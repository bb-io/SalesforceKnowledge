using Salesforce.CmsTests.Base;
using Apps.Salesforce.Cms.Handlers;

namespace Tests.Salesforce.Cms;

[TestClass]
public class DataSourceTests : TestBase
{
    [TestMethod]
    public async Task ArticleDataHandler_ReturnsArticles()
    {
        // Arrange
        var handler = new ArticleDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new(), default);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task LocaleDataHandler_ReturnsActiveLocales()
    {
        // Arrange
        var handler = new LocaleDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new(), default);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UserDataHandler_ReturnsUsers()
    {
        // Arrange
        var handler = new UserDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new(), default);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
