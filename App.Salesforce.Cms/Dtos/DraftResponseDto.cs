using Blackbird.Applications.Sdk.Common;

namespace App.Salesforce.Cms.Dtos;

public class DraftResponseDto
{
    [Display("Output values")]
    public OutputValuesDto OutputValues { get; set; }
}
