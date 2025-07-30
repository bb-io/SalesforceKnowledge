using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Dtos
{
    public class MasterArticleDto
    {

        [Display("Article ID")]
        public string Id { get; set; }

        [Display("Master language")]
        public string MasterLanguage { get; set; }

        [Display("Article number")]
        public string ArticleNumber { get; set; }

        [Display("Attributes")]
        public ArticleAttributes Attributes { get; set; }      

        [Display("Created date")]
        public DateTime CreatedDate { get; set; }

        [Display("Created by ID")]
        public string CreatedById { get; set; }

        [Display("Last modified date")]
        public DateTime? LastModifiedDate { get; set; }

        [Display("Last modified by ID")]
        public string? LastModifiedById { get; set; }

        [Display("Archived date")]
        public DateTime? ArchivedDate { get; set; }

        [Display("Archived by ID")]
        public string? ArchivedById { get; set; }

        [Display("First published date")]
        public DateTime? FirstPublishedDate { get; set; }

        [Display("Last published date")]
        public DateTime? LastPublishedDate { get; set; }

        [Display("Last viewed date")]
        public DateTime? LastViewedDate { get; set; }

        [Display("Total view count")]
        public int TotalViewCount { get; set; }

        [DefinitionIgnore]
        public bool IsDeleted { get; set; }
    }

    public class ArticleAttributes
    {
        [Display("Article type")]
        public string Type { get; set; }

        [Display("URL")]
        public string Url { get; set; }
    }
}
