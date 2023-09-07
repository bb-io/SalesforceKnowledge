using App.Salesforce.Cms.Models.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class ListAllArticlesVersionsResponse
{
    public IEnumerable<ArticleVersionDto> Records { get; set; }
}