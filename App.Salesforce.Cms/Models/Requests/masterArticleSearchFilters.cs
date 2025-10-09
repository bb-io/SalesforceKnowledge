using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Requests
{
    public class masterArticleSearchFilters
    {
        [Display("Created after")]
        public DateTime? CreatedAfter { get; set; }

        [Display("Created before")]
        public DateTime? CreatedBefore { get; set; }

        [Display("Updated after")]
        public DateTime? UpdatedAfter { get; set; }

        [Display("Updated before")]
        public DateTime? UpdatedBefore { get; set; }

        [Display("Published after")]
        public DateTime? PublishedAfter { get; set; }

        [Display("Published before")]
        public DateTime? PublishedBefore { get; set; }
        public bool? Published { get; set; }
        public bool? Archived { get; set; }
    }
}
