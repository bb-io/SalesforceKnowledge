using Apps.Salesforce.Cms.Models.Dtos;

namespace Apps.Salesforce.Cms.Models.Responses;

public record GetArticleCustomContent(List<LayoutItemDto> Items);