using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Requests;

public class UpdateKnowledgeArticleFieldRequest
{
    [Display("Field name")]
    public string FieldName { get; set; }

    [Display("Field value")] 
    public string FieldValue { get; set; }

    [Display("Publish changes")]
    public bool Publish { get; set; }
}