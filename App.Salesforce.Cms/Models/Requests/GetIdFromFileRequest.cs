using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salesforce.Cms.Models.Requests;

public class GetIdFromFileRequest
{
    [Display("Content")]
    public FileReference File { get; set; }
}