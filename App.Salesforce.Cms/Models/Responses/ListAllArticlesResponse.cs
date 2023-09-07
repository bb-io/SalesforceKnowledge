using App.Salesforce.Cms.Models.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class ListAllArticlesResponse
{
    public IEnumerable<ArticleDto> Records { get; set; }
}