namespace App.Salesforce.Cms.Models.Requests;

public class GetArticleContentRequest
{
    public string ArticleId { get; set; }

    public string Locale { get; set; }
}