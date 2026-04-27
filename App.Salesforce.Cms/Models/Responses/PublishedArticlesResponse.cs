using Apps.Salesforce.Cms.Models.Dtos;
using Newtonsoft.Json;

namespace Apps.Salesforce.Cms.Models.Responses;

public class PublishedArticlesResponse
{
    [JsonProperty("articles")]
    public List<PublishedArticleDto> Articles { get; set; } = [];

    [JsonProperty("nextPageUrl")]
    public string? NextPageUrl { get; set; }

    [JsonProperty("currentPageUrl")]
    public string? CurrentPageUrl { get; set; }

    [JsonProperty("pageNumber")]
    public int PageNumber { get; set; }
}