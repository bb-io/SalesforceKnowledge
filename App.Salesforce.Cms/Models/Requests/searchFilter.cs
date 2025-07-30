using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Requests
{
    public class searchFilter
    {
        [Display("Published after")]
        public DateTime? PublishedAfter { get; set; }

        [Display("Published before")]
        public DateTime? PublishedBefore { get; set; }
    }
}
