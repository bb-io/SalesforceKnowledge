using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace App.Salesforce.Cms.Models.Requests;

public class TranslateFromHtmlRequest
{
    public FileReference File { get; set; }

    public string Locale { get; set; }

    [Display("Article ID")]
    public string? ArticleId { get; set; }
}