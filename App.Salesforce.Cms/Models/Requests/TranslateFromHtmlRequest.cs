using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace App.Salesforce.Cms.Models.Requests;

public class TranslateFromHtmlRequest
{
    public File File { get; set; }

    [Display("Article ID")]
    public string ArticleId { get; set; }

    public string Locale { get; set; }
}