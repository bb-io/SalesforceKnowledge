using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Requests
{
    public class CategoryFilterRequest
    {
        [Display("Category name")]
        public string? CategoryName { get; set; }

        [Display("Category group name")]
        public string? GroupName { get; set; }

        [Display("Excluded data categories")]
        public IEnumerable<string>? ExcludedDataCategories { get; set; }
    }
}
