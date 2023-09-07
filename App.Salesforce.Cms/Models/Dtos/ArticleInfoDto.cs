using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Models.Dtos;

public class ArticleInfoDto
{
    [Display("Additional information")]
    public AdditionalInformationDto AdditionalInformation { get; set; }

    [Display("Archived article master version ID")]
    public string ArchivedArticleMasterVersionId { get; set; }

    [Display("Article ID")]
    public string ArticleId { get; set; }

    [Display("Article type")]
    public string ArticleType { get; set; }

    [Display("Draft article master version ID")]
    public string DraftArticleMasterVersionId { get; set; }

    [Display("Master language")]
    public string MasterLanguage { get; set; }

    [Display("Online article master version ID")]
    public string OnlineArticleMasterVersionId { get; set; }
}