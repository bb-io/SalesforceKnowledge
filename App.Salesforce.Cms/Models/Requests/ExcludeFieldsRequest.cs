using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Requests;

public class ExcludeFieldsRequest
{
    [Display("Fields to exclude", Description = "List here the names of the fields you want to exclude from the HTML file")]
    public IEnumerable<string>? Fields { get; set; }
}
