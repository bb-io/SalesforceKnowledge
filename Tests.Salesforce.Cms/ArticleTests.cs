using Salesforce.CmsTests.Base;
using App.Salesforce.Cms.Actions;
using Apps.Salesforce.Cms.Models.Requests;
using Apps.Salesforce.Cms.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;

namespace Tests.Salesforce.Cms;

[TestClass]
public class ArticleTests : TestBase
{
    [TestMethod]
    public async Task SearchAllMasterKnowledge_ReturnsAllArticles()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var input = new SearchMasterArticlesRequest { };

        // Act
        var result = await action.ListAllArticles(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchAllPublishedTranslation_ReturnsPublishedTranslations()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var locale = new LocaleIdentifier { Locale = "en_US" };
        var category = new CategoryFilterRequest { };

        // Act
        var result = await action.ListPublishedArticlesTranslations(locale, category);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchAllPublishedArticles_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var filter = new SearchPublishedArticlesRequest
        {
            //GroupName = "Blackbird" 
        };

        // Act
        var result = await action.ListAllPublishedArticles(filter);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchKnowledgeArticleVersions_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var input = new ArticleIdentifier { ArticleId = "kA067000000E03gCAC" };

        // Act
        var result = await action.ListAllArticlesVersions(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetArticleInfo_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var input = new ArticleIdentifier { ArticleId = "kA067000000E03gCAC" };

        // Act
        var result = await action.GetArticleInfo(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetArticleContentAsObject_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var articleInput = new ArticleIdentifier { ArticleId = "kA067000000E03gCAC" }; 
        var localeInput = new LocaleIdentifier { Locale = "en_US" };

        // Act
        var result = await action.GetArticleContent(articleInput, localeInput);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetArticleCustomContentAsObject_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var articleInput = new ArticleIdentifier { ArticleId = "kA067000000E03gCAC" }; 
        var localeInput = new LocaleIdentifier { Locale = "en_US" };

        // Act
        var result = await action.GetArticleCustomContent(articleInput, localeInput);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetArticleContentAsHtml_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var input = new DownloadArticleRequest
        {
            ContentId = "kA0J5000000g47hKAA",
        };
        var locale = new LocaleIdentifier { Locale = "en_US" };

        // Act
        var result = await action.GetArticleContentAsHtml(input, locale);

        // Assert
        Console.WriteLine(result.Content.Name);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetArticlesNotTranslated_ReturnsNonTranslatedArticles()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var locale = new LocaleIdentifier { Locale = "en_US" };

        // Act
        var result = await action.GetArticlesNotTranslated(locale);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TranslateFromHtml_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var input = new UploadArticleRequest
        {
            Locale = "nl_NL",
            Content = new FileReference { Name = "test blueprints.html" },
            Publish = true,
        };

        // Act
        var result = await action.TranslateFromHtml(input);
        
        // Assert
        Console.WriteLine(result.Content.Name);
    }

    [TestMethod]
    public async Task SubmitKnowledgeArticleToTranslation_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var article = new ArticleIdentifier { ArticleId = "kA067000000E03gCAC" };
        var locale = new LocaleIdentifier { Locale = "en_US" }; 
        var input = new SubmitToTranslationRequest
        {
            AssigneeId = "0056700000Dd6t7AAB",
        };

        // Act
        await action.SubmitToTranslation(article, input, locale); 
    }

    [TestMethod]
    public async Task PublishKnowledgeTranslation_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var article = new ArticleIdentifier { ArticleId = "ka0J5000000fy2ZIAQ" };
        var locale = new LocaleIdentifier { Locale = "en_US" };
        var input = new PublishKnowledgeTranslationRequest { };

        // Act
        await action.PublishKnowledgeTranslation(article, input, locale);
    }

    [TestMethod]
    public async Task CreateDraftKnowledgeTranslation_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var articleInput = new ArticleIdentifier { ArticleId = "ka067000000EMXVAA4" };
        var input = new CreateArticleDraftRequest { };
        var locale = new LocaleIdentifier { Locale = "en_US" };

        // Act
        var result = await action.CreatedArticleDraft(articleInput, input, locale);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetKnowledgeSettings_ReturnsLanguageSettings()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);

        // Act
        var result = await action.GetKnowledgeSettings();

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateKnowledgeArticleField_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var articleInput = new ArticleIdentifier { ArticleId = "kA067000000E03gCAC" };
        var input = new UpdateKnowledgeArticleFieldRequest
        {
            FieldName = "title",
            FieldValue = "New Title 03/21",
            Publish = true,
        };
        var locale = new LocaleIdentifier { Locale = "en_US" };

        // Act
        await action.UpdateKnowledgeArticleField(articleInput, input, locale);
    }

    [TestMethod]
    public async Task GetArticleIdFromHtmlFile_IsSuccess()
    {
        // Arrange
        var action = new ArticleActions(InvocationContext, FileManager);
        var input = new GetIdFromFileRequest { File = new FileReference { Name = "test.html" } };

        // Act
        var result = await action.GetArticleIdFromHtmlFile(input);

        // Assert
        Console.WriteLine(result);
        Assert.IsNotNull(result);
    }
}