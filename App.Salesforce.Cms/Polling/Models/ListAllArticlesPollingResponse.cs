using Apps.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Polling.Models;

public class ListAllArticlesPollingResponse
{
    [Display("Total count")]
    public int TotalCount { get; set; }

    [Display("Articles")]
    public MasterArticleDto[] Records { get; set; }

    public ListAllArticlesPollingResponse(MasterArticleDto[] records)
    {
        Records = records;
        TotalCount = records?.Length ?? 0;
    }
}