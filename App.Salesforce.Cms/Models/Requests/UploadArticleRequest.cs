using Apps.Salesforce.Cms.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Salesforce.Cms.Models.Requests;

public class UploadArticleRequest : IUploadContentInput
{
    [Display("Content")]
    public FileReference Content { get; set; }

    [Display("Locale")]
    public string Locale { get; set; }

    [Display("Article ID"), DataSource(typeof(ArticleDataHandler))]
    public string? ContentId { get; set; }

    [Display("Publish changes")]
    public bool Publish { get; set; }
}