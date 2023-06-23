using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Dtos
{
    public class DraftResponseDto
    {
        public OutputValues OutputValues { get; set; }
    }

    public class OutputValues
    {
        public string DraftId { get; set; }
    }
}
