using Apps.Salesforce.Cms.Models.Dtos;

namespace Apps.Salesforce.Cms.Models.Responses;

public class ListAllArticlesVersionsResponse
{
    public IEnumerable<ArticleVersionDto> Records { get; set; }
}