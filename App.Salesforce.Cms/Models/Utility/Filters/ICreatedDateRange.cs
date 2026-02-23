namespace Apps.Salesforce.Cms.Models.Utility.Filters;

public interface ICreatedDateRange : IDateFilter
{
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}
