namespace App.Salesforce.Cms.Models.Requests;

public class CreateArticleDraftRequest : ArticleRequest
{
    public string Locale { get; set; }
}