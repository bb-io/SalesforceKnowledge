using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Polling.Models
{
    public class VisibilityFilterRequest
    {
        [Display("Visible in public knowledge base")]
        public bool? IsVisibleInPkb { get; set; }

        [Display("Visible to customer")]
        public bool? IsVisibleInCsp { get; set; }
    }
}
