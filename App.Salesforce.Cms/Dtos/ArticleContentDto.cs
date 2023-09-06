using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Dtos;

public class ArticleContentDto
{
    [Display("All view count")]
    public int AllViewCount { get; set; }

    [Display("All view score")]
    public double AllViewScore { get; set; }

    [Display("App down vote count")]
    public int AppDownVoteCount { get; set; }

    [Display("App up vote count")]
    public int AppUpVoteCount { get; set; }

    [Display("App view count")]
    public int AppViewCount { get; set; }

    [Display("App view score")]
    public double AppViewScore { get; set; }

    [Display("Article number")]
    public string ArticleNumber { get; set; }

    [Display("Article type")]
    public string ArticleType { get; set; }

    [Display("CSP down vote count")]
    public int CspDownVoteCount { get; set; }

    [Display("CSP up vote count")]
    public int CspUpVoteCount { get; set; }

    [Display("CSP view count")]
    public int CspViewCount { get; set; }

    [Display("CSP view score")]
    public double CspViewScore { get; set; }

    [Display("ID")]
    public string Id { get; set; }

    [Display("Layout items")]
    public IEnumerable<LayoutItemDto> LayoutItems { get; set; }

    [Display("PKB down vote count")]
    public int PkbDownVoteCount { get; set; }

    [Display("PKB up vote count")]
    public int PkbUpVoteCount { get; set; }

    [Display("PKB view count")]
    public int PkbViewCount { get; set; }

    [Display("PKB view score")]
    public double PkbViewScore { get; set; }

    [Display("PRM down vote count")]
    public int PrmDownVoteCount { get; set; }

    [Display("PRM up vote count")]
    public int PrmUpVoteCount { get; set; }

    [Display("PRM view count")]
    public int PrmViewCount { get; set; }

    [Display("PRM view score")]
    public double PrmViewScore { get; set; }

    [Display("Title")]
    public string Title { get; set; }

    [Display("URL")]
    public string Url { get; set; }

    [Display("URL name")]
    public string UrlName { get; set; }

    [Display("Version number")]
    public int VersionNumber { get; set; }
}