using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Dtos;

public class KnowledgeSettingsDto
{
    [Display("Default language")]
    public string DefaultLanguage { get; set; }
    
    [Display("Knowledge enabled")]
    public bool KnowledgeEnabled { get; set; }
    
    public List<LanguageDto> Languages { get; set; }
}
