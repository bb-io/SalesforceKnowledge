using Newtonsoft.Json;

namespace App.Salesforce.Cms.Models.Dtos;

public class SalesforceErrorDto
{
    [JsonProperty("error")]
    public string Error { get; set; } = string.Empty;

    [JsonProperty("error_description")]
    public string ErrorDescription { get; set; } = string.Empty;
}
