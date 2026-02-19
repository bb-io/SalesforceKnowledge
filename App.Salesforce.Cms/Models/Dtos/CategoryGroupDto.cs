using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class CategoryGroupDto
{
    [Display("Group label")]
    public string GroupLabel { get; set; }

    [Display("Group name")]
    public string GroupName { get; set; }

    [Display("Selected categories")]
    public IEnumerable<CategoryDto> SelectedCategories { get; set; }
}
