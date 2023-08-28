using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class PublishKnowledgeTranslationRequest
{
    [Display("Article ID")]
    public string ArticleId { get; set; }

    public string Locale { get; set; }
}