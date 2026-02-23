using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class ArticleDto : IDownloadContentInput
{
    [Display("Article ID")]
    public string ContentId { get; set; }

    [Display("Title")]
    public string Title { get; set; }

    [Display("Article number")] 
    public string ArticleNumber { get; set; }

    [Display("Master language")] 
    public string MasterLanguage { get; set; }

    [Display("Total view count")] 
    public int TotalViewCount { get; set; }

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

    [Display("Category groups")]
    public IEnumerable<CategoryGroupDto> CategoryGroups { get; set; }

    [Display("Visible in public knowledge base")]
    public bool? IsVisibleInPkb { get; set; }

    [Display("Visible to customer")]
    public bool? IsVisibleInCsp { get; set; }

    public ArticleDto() { }

    public ArticleDto(PublishedArticleDto article, string masterLanguage)
    {
        ContentId = article.Id;
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
        CategoryGroups = article.CategoryGroups?.Select(cg => new CategoryGroupDto
        {
            GroupLabel = cg.GroupLabel,
            GroupName = cg.GroupName,
            SelectedCategories = cg.SelectedCategories?.Select(sc => new CategoryDto
            {
                CategoryLabel = sc.CategoryLabel,
                CategoryName = sc.CategoryName,
                Url = sc.Url
            }) ?? []
        }) ?? [];

        IsVisibleInPkb = article.IsVisibleInPkb;
        IsVisibleInCsp = article.IsVisibleInCsp;
    }
}