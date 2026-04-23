namespace Apps.Salesforce.Cms.Models.Dtos;

public record CustomContentFieldDto(string Label, string Value, int? MaxLength = null, string? CustomFieldId = null);
