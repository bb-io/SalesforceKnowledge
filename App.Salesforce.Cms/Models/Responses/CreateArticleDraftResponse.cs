using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Responses;

public record CreateArticleDraftResponse
{
    [Display("Draft version ID")]
    public string DraftVersionId { get; set; }
}