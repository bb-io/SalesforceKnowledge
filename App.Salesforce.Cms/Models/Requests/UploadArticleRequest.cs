using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace App.Salesforce.Cms.Models.Requests;

public class UploadArticleRequest : IUploadContentInput
{
    [Display("Content")]
    public FileReference Content { get; set; }

    [Display("Locale")]
    public string Locale { get; set; }

    [Display("Article ID")]
    public string? ContentId { get; set; }

    [Display("Publish changes")]
    public bool Publish { get; set; }
}