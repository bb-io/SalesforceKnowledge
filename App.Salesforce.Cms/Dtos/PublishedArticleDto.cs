namespace App.Salesforce.Cms.Dtos;

public class PublishedArticlesResponse
{
    public IEnumerable<PublishedArticleDto> Articles { get; set; }
}
public class PublishedArticleDto
{
    public string ArticleNumber { get; set; }
    public int DownVoteCount { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public int UpVoteCount { get; set; }
    public string Url { get; set; }
    public string UrlName { get; set; }
    public int ViewCount { get; set; }
    public double ViewScore { get; set; }
}