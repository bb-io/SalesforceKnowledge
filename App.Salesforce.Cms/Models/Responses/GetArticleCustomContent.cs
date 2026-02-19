using Apps.Salesforce.Cms.Models.Dtos;

namespace Apps.Salesforce.Cms.Models.Responses;

public class GetArticleCustomContent
{
    public IEnumerable<LayoutItemDto> Items { get; set; }
}