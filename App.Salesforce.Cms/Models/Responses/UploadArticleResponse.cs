using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salesforce.Cms.Models.Responses;

public class UploadArticleResponse
{
    [Display("Content")]
    public FileReference Content { get; set; }
}