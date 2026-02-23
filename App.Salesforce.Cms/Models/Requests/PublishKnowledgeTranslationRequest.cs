using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Requests;

public class PublishKnowledgeTranslationRequest
{    
    [Display("Publish date")]
    public DateTime? PubDate { get; set; }
}