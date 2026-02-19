using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace App.Salesforce.Cms.Models.Responses;

public class DownloadArticleResponse(FileReference content) : IDownloadContentOutput
{
    [Display("Content")]
    public FileReference Content { get; set; } = content;
}