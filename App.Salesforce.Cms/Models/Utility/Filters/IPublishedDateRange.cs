namespace Apps.Salesforce.Cms.Models.Utility.Filters;

public interface IPublishedDateRange : IDateFilter
{
    public DateTime? PublishedAfter { get; set; }
    public DateTime? PublishedBefore { get; set; }
}