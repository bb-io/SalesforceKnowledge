using App.Salesforce.Cms.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Models.Responses
{
    public class ListAllArticlesResponse
    {
        public IEnumerable<ArticleDto> Records { get; set; }
    }
}
