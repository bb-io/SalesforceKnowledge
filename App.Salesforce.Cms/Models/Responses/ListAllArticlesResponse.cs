using App.Salesforce.Cms.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class ListAllArticlesResponse
{
    public IEnumerable<ArticleDto> Records { get; set; }
}