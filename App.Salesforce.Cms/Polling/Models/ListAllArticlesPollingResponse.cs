using Apps.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Polling.Models;

public class ListAllArticlesPollingResponse(List<MasterArticleDto> records)
{
    [Display("Total count")]
    public int TotalCount { get; set; } = records.Count;

    [Display("Articles")]
    public List<MasterArticleDto> Records { get; set; } = records;
}