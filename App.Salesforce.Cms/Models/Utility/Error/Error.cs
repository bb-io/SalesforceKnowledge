namespace Apps.Salesforce.Cms.Models.Utility.Error;

using Newtonsoft.Json;

public class Error
{
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;
}