using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Models.Requests
{
    public class SubmitToTranslationRequest
    {
        [Display("Article ID")]
        public string ArticleId { get; set; }

        public string Locale { get; set; }


        [Display("Assignee ID")]
        public string AssigneeId { get; set; }
    }
}
