using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Salesforce.Cms.Polling.Models
{
    public class KnowledgeArticle
    {
        public ArticleAttributes attributes { get; set; }

        public string Id { get; set; }

        [Display("Is deleted")]
        public bool IsDeleted { get; set; }

        [Display("Create date")]
        public string CreatedDate { get; set; }

        [Display("Created by ID")]
        public string CreatedById { get; set; }

        [Display("Last modified date")]
        public string LastModifiedDate { get; set; }

        [Display("Last modified by ID")]
        public string LastModifiedById { get; set; }

        [Display("System modstamp")]
        public string SystemModstamp { get; set; }

        public string MasterLanguage { get; set; }
        public string ArticleNumber { get; set; }
        public string ArchivedDate { get; set; }
        public string ArchivedById { get; set; }
        public string FirstPublishedDate { get; set; }
        public string LastPublishedDate { get; set; }
        public int CaseAssociationCount { get; set; }
        public string LastViewedDate { get; set; }
        public string LastReferencedDate { get; set; }
        public string MigratedToFromArticle { get; set; }
        public int TotalViewCount { get; set; }
    }
    public class ArticleAttributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public record ListArticlesResponse(IEnumerable<KnowledgeArticle> Articles);

}
