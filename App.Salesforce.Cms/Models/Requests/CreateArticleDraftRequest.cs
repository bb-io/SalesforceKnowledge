using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Models.Requests
{
    public class CreateArticleDraftRequest
    {
        public string ArticleId { get; set; }

        public string Locale { get; set; }
    }
}
