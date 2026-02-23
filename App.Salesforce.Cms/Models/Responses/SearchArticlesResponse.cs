using Apps.Salesforce.Cms.Models.Dtos;

namespace Apps.Salesforce.Cms.Models.Responses;

public record SearchArticlesResponse(List<ArticleDto> Items);