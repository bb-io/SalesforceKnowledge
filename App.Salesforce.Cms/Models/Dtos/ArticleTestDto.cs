using Newtonsoft.Json;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class ArticleTestDto
{
    [JsonProperty("KnowledgeArticleId")]
    public string Id { get; set; }

    [JsonProperty("Title")]
    public string Title { get; set; }
}
