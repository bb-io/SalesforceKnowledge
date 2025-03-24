using App.Salesforce.Cms.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Salesforce.Cms.Polling.Models
{   public class ListAllArticlesPollingResponse
    {
        [Display("Total count")]
        public int totalSize { get; set; }

        [Display("Is done")]
        public bool done { get; set; }

        [Display("Records")]
        public KnowledgeArticle[] Records { get; set; }

        public ListAllArticlesPollingResponse(KnowledgeArticle[] records)
        {
            Records = records;
            totalSize = records?.Length ?? 0;
            done = true; 
        }

        public ListAllArticlesPollingResponse() { }
    }
}
