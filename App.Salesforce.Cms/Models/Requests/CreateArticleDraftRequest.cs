namespace App.Salesforce.Cms.Models.Requests;

public class CreateArticleDraftRequest
{
    public string Locale { get; set; }
    
    public bool? Unpublish { get; set; }
}