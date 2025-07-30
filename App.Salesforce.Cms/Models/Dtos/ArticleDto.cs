using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Dtos;

public class ArticleDto
{
    [Display("ID")] public string Id { get; set; }

    public string Title { get; set; }

    [Display("Article number")] public string ArticleNumber { get; set; }

    [Display("Master language")] public string MasterLanguage { get; set; }

    [Display("Total view count")] public int TotalViewCount { get; set; }

    [Display("Down vote count")]
    public int DownVoteCount { get; set; }

    [Display("Up vote count")]
    public int UpVoteCount { get; set; }

    [Display("URL")]
    public string? Url { get; set; }

    [Display("URL name")]
    public string? UrlName { get; set; }

    [Display("View score")]
    public double ViewScore { get; set; }

    [Display("Last published date")]
    public DateTime LastPublishedDate { get; set; }

    [Display("Summary")]
    public string? Summary { get; set; }

    public ArticleDto(PublishedArticleDto article, string masterLanguage)
    {
        Id = article.Id;
        Title = article.Title;
        ArticleNumber = article.ArticleNumber;
        MasterLanguage = masterLanguage;
        TotalViewCount = article.ViewCount;
        DownVoteCount = article.DownVoteCount;
        UpVoteCount = article.UpVoteCount;
        Url = article.Url;
        UrlName = article.UrlName;
        ViewScore = article.ViewScore;
        LastPublishedDate = article.LastPublishedDate;
        Summary = article.Summary;
    }

    public ArticleDto()
    {
    }
}