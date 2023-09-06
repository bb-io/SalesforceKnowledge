using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Dtos;

public class AdditionalInformationDto
{
    [Display("Can Aarchive")]
    public bool CanArchive { get; set; }

    [Display("Can delete")]
    public bool CanDelete { get; set; }

    [Display("Can edit")]
    public bool CanEdit { get; set; }

    [Display("Can publish")]
    public bool CanPublish { get; set; }

    [Display("Can unpublish")]
    public bool CanUnpublish { get; set; }

    [Display("Has archived versions")]
    public bool HasArchivedVersions { get; set; }

    [Display("Has translations")]
    public bool HasTranslations { get; set; }
}