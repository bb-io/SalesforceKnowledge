namespace Apps.Salesforce.Cms.Models.Dtos;

public record ObjectFieldDto
{
    public string QualifiedApiName { get; set; }
    public string DurableId { get; set; }
    public int Length { get; set; }
}