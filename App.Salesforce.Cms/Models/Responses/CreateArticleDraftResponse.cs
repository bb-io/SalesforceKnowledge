using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Responses;

public class CreateArticleDraftResponse
{
    [Display("Draft version ID")]
    public string DraftVersionId { get; set; }
}