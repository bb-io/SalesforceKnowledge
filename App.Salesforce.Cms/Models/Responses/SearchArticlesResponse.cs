using Apps.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Salesforce.Cms.Models.Responses;

public record SearchArticlesResponse(List<ArticleDto> Items) : IMultiDownloadableContentOutput<ArticleDto>
{
    public List<ArticleDto> Items { get; set; } = Items;
};