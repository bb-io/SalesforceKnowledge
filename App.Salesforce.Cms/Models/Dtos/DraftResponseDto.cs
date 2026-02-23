using Blackbird.Applications.Sdk.Common;

namespace Apps.Salesforce.Cms.Models.Dtos;

public class DraftResponseDto
{
    [Display("Output values")]
    public OutputValuesDto OutputValues { get; set; }
}
