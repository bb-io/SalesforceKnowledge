using Apps.Salesforce.Cms.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salesforce.Cms.Models.Identifiers;

public class LocaleIdentifier
{
    [Display("Locale"), DataSource(typeof(LocaleDataHandler))]
    public string Locale { get; set; }
}