using Apps.Salesforce.Cms.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Salesforce.Cms.Models.Requests;

public class DownloadArticleRequest : IDownloadContentInput
{
    [Display("Article ID"), DataSource(typeof(ArticleDataHandler))]
    public string ContentId { get; set; }

    [Display("Fields to exclude", Description = "Fields you want to exclude from the HTML file")]
    public IEnumerable<string>? FieldsToExclude { get; set; }
}
