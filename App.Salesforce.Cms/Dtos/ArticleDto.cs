using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Dtos
{
    public class ArticleDto
    {
        public ArticleDto(PublishedArticleDto article, string masterLanguage) {
            Id = article.Id;
            ArticleNumber = article.ArticleNumber;
            MasterLanguage = masterLanguage;
            TotalViewCount = article.ViewCount;
        }
        public string Id { get; set; }
        public string ArticleNumber { get; set; }
        public string MasterLanguage { get; set; }
        public int TotalViewCount { get; set; }
    }
}
