namespace App.Salesforce.Cms.Models.Requests;

public class GetArticleContentRequest : ArticleRequest
{
    public string Locale { get; set; }
}