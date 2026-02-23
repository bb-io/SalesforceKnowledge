namespace Apps.Salesforce.Cms.Models.Utility.Filters;

public interface IUpdatedDateRange : IDateFilter
{
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }
}
