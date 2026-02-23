using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class ArticleAttributes
{
    [Display("Article type")]
    public string Type { get; set; } = string.Empty;

    [Display("Article URL")]
    public string Url { get; set; } = string.Empty;
}
