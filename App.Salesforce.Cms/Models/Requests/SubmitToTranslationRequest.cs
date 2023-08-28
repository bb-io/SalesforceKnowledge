using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class SubmitToTranslationRequest
{
    [Display("Article ID")]
    public string ArticleId { get; set; }

    public string Locale { get; set; }


    [Display("Assignee ID")]
    public string AssigneeId { get; set; }
}