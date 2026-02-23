using Apps.Salesforce.Cms.Models.Dtos;

namespace Apps.Salesforce.Cms.Models.Responses;

public class PublishedArticlesResponse
{
    public IEnumerable<PublishedArticleDto> Articles { get; set; }
}