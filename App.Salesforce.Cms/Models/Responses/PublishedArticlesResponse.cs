using App.Salesforce.Cms.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class PublishedArticlesResponse
{
    public IEnumerable<PublishedArticleDto> Articles { get; set; }
}