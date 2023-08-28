namespace App.Salesforce.Cms.Dtos;

public class ArticleVersionDto
{
    public string Id { get; set; }
    public string KnowledgeArticleId { get; set; }
    public string OwnerId { get; set; }
    public bool IsDeleted { get; set; }
    public string ValidationStatus { get; set; }
    public string PublishStatus { get; set; }
    public int VersionNumber { get; set; }
    public bool IsLatestVersion { get; set; }
    public bool IsVisibleInApp { get; set; }
    public bool IsVisibleInPkb { get; set; }
    public bool IsVisibleInCsp { get; set; }
    public bool IsVisibleInPrm { get; set; }
    public string CreatedById { get; set; }
    public string LastModifiedById { get; set; }
    public bool IsMasterLanguage { get; set; }
    public string Language { get; set; }
    public string Title { get; set; }
    public string UrlName { get; set; }
    public string ArticleNumber { get; set; }
    public int ArticleCaseAttachCount { get; set; }
    public string ArticleCreatedById { get; set; }
    public string ArticleMasterLanguage { get; set; }
    public int ArticleTotalViewCount { get; set; }
    public bool IsOutOfDate { get; set; }
    public string MasterVersionId { get; set; }
    public string RecordTypeId { get; set; }
    public string AssignedToId { get; set; }
    public string AssignedById { get; set; }
}