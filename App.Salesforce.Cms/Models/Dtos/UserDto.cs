using Newtonsoft.Json;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class UserDto
{
    [JsonProperty("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("Email")]
    public string Email { get; set; } = string.Empty;
}
