using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class UpdateKnowledgeArticleFieldRequest : ArticleRequest
{
    public string Locale { get; set; }

    [Display("Field name")] public string FieldName { get; set; }

    [Display("Field value")] public string FieldValue { get; set; }
}