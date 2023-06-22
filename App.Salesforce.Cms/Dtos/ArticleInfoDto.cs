using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Dtos
{
    public class ArticleInfoDto
    {
        public AdditionalInformation AdditionalInformation { get; set; }
        public string ArchivedArticleMasterVersionId { get; set; }
        public string ArticleId { get; set; }
        public string ArticleType { get; set; }
        public string DraftArticleMasterVersionId { get; set; }
        public string MasterLanguage { get; set; }
        public string OnlineArticleMasterVersionId { get; set; }
    }

    public class AdditionalInformation
    {
        public bool CanArchive { get; set; }
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public bool CanPublish { get; set; }
        public bool CanUnpublish { get; set; }
        public bool HasArchivedVersions { get; set; }
        public bool HasTranslations { get; set; }
    }
}
