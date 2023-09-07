using App.Salesforce.Cms.Models.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class GetArticleCustomContent
{
    public IEnumerable<LayoutItemDto> Items { get; set; }
}