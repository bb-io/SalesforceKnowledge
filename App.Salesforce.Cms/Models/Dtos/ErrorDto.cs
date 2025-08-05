namespace Apps.Salesforce.Cms.Models.Dtos;

using Newtonsoft.Json;

public class ErrorDto
{
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;
}