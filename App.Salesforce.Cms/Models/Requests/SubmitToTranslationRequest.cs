using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Requests;

public class SubmitToTranslationRequest : ArticleRequest
{
    public string Locale { get; set; }
    [Display("Assignee ID")] public string AssigneeId { get; set; }
    
    [Display("Due date")]
    public DateTime? DueDate { get; set; }
    
    [Display("Send email notification")]
    public bool? SendEmailNotification { get; set; }
}