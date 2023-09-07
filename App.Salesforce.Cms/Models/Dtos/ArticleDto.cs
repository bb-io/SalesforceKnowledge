using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Dtos;

public class ArticleDto
{
    public ArticleDto(PublishedArticleDto article, string masterLanguage) {
        Id = article.Id;
        Title = article.Title;
        ArticleNumber = article.ArticleNumber;
        MasterLanguage = masterLanguage;
        TotalViewCount = article.ViewCount;
    }
    
    [Display("ID")]
    public string Id { get; set; }
    
    public string Title { get; set; }
    
    [Display("Article number")]
    public string ArticleNumber { get; set; }
    
    [Display("Master language")]
    public string MasterLanguage { get; set; }
    
    [Display("Total view count")]
    public int TotalViewCount { get; set; }
}