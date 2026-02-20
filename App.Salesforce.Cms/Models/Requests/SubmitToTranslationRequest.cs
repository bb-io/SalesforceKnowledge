using Apps.Salesforce.Cms.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salesforce.Cms.Models.Requests;

public class SubmitToTranslationRequest
{
    [Display("Assignee ID"), DataSource(typeof(UserDataHandler))] 
    public string AssigneeId { get; set; }
    
    [Display("Due date")]
    public DateTime? DueDate { get; set; }
    
    [Display("Send email notification")]
    public bool? SendEmailNotification { get; set; }
}