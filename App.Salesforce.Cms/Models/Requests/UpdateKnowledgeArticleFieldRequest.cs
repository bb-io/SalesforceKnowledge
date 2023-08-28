using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class UpdateKnowledgeArticleFieldRequest
{

    [Display("Article ID")]
    public string ArticleId { get; set; }

    public string Locale { get; set; }

    [Display("Field name")]
    public string FieldName { get; set; }

    [Display("Field value")]
    public string FieldValue { get; set; }
}