using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class CreateArticleDraftRequest
{
    [Display("Article ID")]
    public string ArticleId { get; set; }

    public string Locale { get; set; }
}