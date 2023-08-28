using App.Salesforce.Cms.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class GetArticleCustomContent
{
    public IEnumerable<LayoutItem> Items { get; set; }
}