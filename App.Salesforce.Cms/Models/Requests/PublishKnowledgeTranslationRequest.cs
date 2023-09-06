namespace App.Salesforce.Cms.Models.Requests;

public class PublishKnowledgeTranslationRequest : ArticleRequest
{
    public string Locale { get; set; }
}