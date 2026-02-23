using Newtonsoft.Json;

namespace Apps.Salesforce.Cms.Models.Utility.Error;

public class ErrorResponse
{
    [JsonProperty("errors")]
    public List<Error> Errors { get; set; } = [];
}
