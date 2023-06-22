using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Dtos
{
    public class ArticleContentDto
    {
        public int AllViewCount { get; set; }
        public double AllViewScore { get; set; }
        public int AppDownVoteCount { get; set; }
        public int AppUpVoteCount { get; set; }
        public int AppViewCount { get; set; }
        public double AppViewScore { get; set; }
        public string ArticleNumber { get; set; }
        public string ArticleType { get; set; }
        public int CspDownVoteCount { get; set; }
        public int CspUpVoteCount { get; set; }
        public int CspViewCount { get; set; }
        public double CspViewScore { get; set; }
        public string Id { get; set; }
        public List<LayoutItem> LayoutItems { get; set; }
        public int PkbDownVoteCount { get; set; }
        public int PkbUpVoteCount { get; set; }
        public int PkbViewCount { get; set; }
        public double PkbViewScore { get; set; }
        public int PrmDownVoteCount { get; set; }
        public int PrmUpVoteCount { get; set; }
        public int PrmViewCount { get; set; }
        public double PrmViewScore { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string UrlName { get; set; }
        public int VersionNumber { get; set; }
    }

    public class LayoutItem
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }
}
