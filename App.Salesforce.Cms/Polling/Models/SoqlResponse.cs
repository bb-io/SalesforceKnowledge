using Newtonsoft.Json;

namespace Apps.Salesforce.Cms.Polling.Models
{
    public class SoqlResponse<T>
    {
        [JsonProperty("records")]
        public IEnumerable<T> Records { get; set; } = Enumerable.Empty<T>();
    }

    public class KnowledgeArticleVisibilityDto
    {
        [JsonProperty("KnowledgeArticleId")]
        public string KnowledgeArticleId { get; set; }

        [JsonProperty("IsVisibleInPkb")]
        public bool? IsVisibleInPkb { get; set; }

        [JsonProperty("IsVisibleInCsp")]
        public bool? IsVisibleInCsp { get; set; }

        [JsonProperty("Id")]
        public string VersionId { get; set; }
    }
}
