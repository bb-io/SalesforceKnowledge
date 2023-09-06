using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class ArticleRequest
{
    [Display("Article ID")]
    public string ArticleId { get; set; }
}