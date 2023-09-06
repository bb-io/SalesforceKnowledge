using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Dtos;

public class ArticleVersionDto
{
    [Display("ID")]
    public string Id { get; set; }

    [Display("Knowledge article ID")]
    public string KnowledgeArticleId { get; set; }

    [Display("Owner ID")]
    public string OwnerId { get; set; }

    [Display("Is deleted")]
    public bool IsDeleted { get; set; }

    [Display("Validation status")]
    public string ValidationStatus { get; set; }

    [Display("Publish status")]
    public string PublishStatus { get; set; }

    [Display("Version number")]
    public int VersionNumber { get; set; }

    [Display("Is latest version")]
    public bool IsLatestVersion { get; set; }

    [Display("Is visible in App")]
    public bool IsVisibleInApp { get; set; }

    [Display("Is visible in PKB")]
    public bool IsVisibleInPkb { get; set; }

    [Display("Is visible in CSP")]
    public bool IsVisibleInCsp { get; set; }

    [Display("Is visible in PRM")]
    public bool IsVisibleInPrm { get; set; }

    [Display("Created by ID")]
    public string CreatedById { get; set; }

    [Display("Last modified by ID")]
    public string LastModifiedById { get; set; }

    [Display("Is master language")]
    public bool IsMasterLanguage { get; set; }

    [Display("Language")]
    public string Language { get; set; }

    [Display("Title")]
    public string Title { get; set; }

    [Display("URL name")]
    public string UrlName { get; set; }

    [Display("Article number")]
    public string ArticleNumber { get; set; }

    [Display("Article case attach count")]
    public int ArticleCaseAttachCount { get; set; }

    [Display("Article created by ID")]
    public string ArticleCreatedById { get; set; }

    [Display("Article master language")]
    public string ArticleMasterLanguage { get; set; }

    [Display("Article total view count")]
    public int ArticleTotalViewCount { get; set; }

    [Display("Is out of Date")]
    public bool IsOutOfDate { get; set; }

    [Display("Master version ID")]
    public string MasterVersionId { get; set; }

    [Display("Record type ID")]
    public string RecordTypeId { get; set; }

    [Display("Assigned to ID")]
    public string AssignedToId { get; set; }

    [Display("Assigned by ID")]
    public string AssignedById { get; set; }
}