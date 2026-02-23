using Apps.Salesforce.Cms.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salesforce.Cms.Models.Identifiers;

public class ArticleIdentifier
{
    [Display("Article ID"), DataSource(typeof(ArticleDataHandler))]
    public string ArticleId { get; set; }
}
