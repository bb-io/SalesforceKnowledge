using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Models.Requests
{
    public class UpdateKnowledgeArticleFieldRequest
    {

        [Display("Article ID")]
        public string ArticleId { get; set; }

        public string Locale { get; set; }

        [Display("Field name")]
        public string FieldName { get; set; }

        [Display("Field value")]
        public string FieldValue { get; set; }
    }
}
