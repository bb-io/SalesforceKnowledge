using App.Salesforce.Cms.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class ListAllArticlesVersionsResponse
{
    public IEnumerable<ArticleVersionDto> Records { get; set; }
}