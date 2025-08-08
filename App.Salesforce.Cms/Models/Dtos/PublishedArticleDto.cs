using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Dtos;

public class PublishedArticleDto
{
    [Display("Article number")]
    public string ArticleNumber { get; set; }

    [Display("Down vote сount")]
    public int DownVoteCount { get; set; }

    [Display("ID")]
    public string Id { get; set; }

    [Display("Title")]
    public string Title { get; set; }

    [Display("Up vote count")]
    public int UpVoteCount { get; set; }

    [Display("URL")]
    public string Url { get; set; }

    [Display("URL name")]
    public string UrlName { get; set; }

    [Display("View count")]
    public int ViewCount { get; set; }

    [Display("View score")]
    public double ViewScore { get; set; }

    [Display("Last published date")]
    public DateTime LastPublishedDate { get; set; }

    [Display("Summary")]
    public string? Summary { get; set; }

    [Display("Category groups")]
    public IEnumerable<CategoryGroupDto> CategoryGroups { get; set; }
}