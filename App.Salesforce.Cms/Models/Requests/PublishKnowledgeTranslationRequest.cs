using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class PublishKnowledgeTranslationRequest
{
    public string Locale { get; set; }
    
    [Display("Publish date")]
    public DateTime? PubDate { get; set; }
}