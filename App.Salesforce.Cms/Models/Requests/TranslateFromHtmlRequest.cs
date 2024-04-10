using Blackbird.Applications.Sdk.Common.Files;

namespace App.Salesforce.Cms.Models.Requests;

public class TranslateFromHtmlRequest : ArticleRequest
{
    public FileReference File { get; set; }

    public string Locale { get; set; }
}