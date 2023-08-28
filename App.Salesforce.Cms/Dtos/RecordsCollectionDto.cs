namespace App.Salesforce.Cms.Dtos;

public class RecordsCollectionDto<T>
{
    public IEnumerable<T> Records { get; set; }
}