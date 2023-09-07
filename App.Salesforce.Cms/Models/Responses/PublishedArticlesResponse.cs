using App.Salesforce.Cms.Models.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class PublishedArticlesResponse
{
    public IEnumerable<PublishedArticleDto> Articles { get; set; }
}