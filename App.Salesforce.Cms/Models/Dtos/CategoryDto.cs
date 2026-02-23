using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class CategoryDto
{
    [Display("Category label")]
    public string CategoryLabel { get; set; }

    [Display("Category name")]
    public string CategoryName { get; set; }

    [Display("Category URL")]
    public string Url { get; set; }
}