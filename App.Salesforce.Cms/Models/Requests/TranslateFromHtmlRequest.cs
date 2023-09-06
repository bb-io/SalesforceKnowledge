using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace App.Salesforce.Cms.Models.Requests;

public class TranslateFromHtmlRequest : ArticleRequest
{
    public File File { get; set; }

    public string Locale { get; set; }
}