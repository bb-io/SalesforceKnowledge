namespace Apps.Salesforce.Cms.Models.Utility.Wrappers;

public class RecordWrapper<T>
{
    public IEnumerable<T> Records { get; set; } = [];
}
