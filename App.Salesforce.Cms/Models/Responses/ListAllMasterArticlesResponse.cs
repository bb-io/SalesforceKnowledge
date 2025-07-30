using Apps.Salesforce.Cms.Models.Dtos;

namespace App.Salesforce.Cms.Models.Responses;

public class ListAllMasterArticlesResponse
{
    public IEnumerable<MasterArticleDto> Records { get; set; }
}