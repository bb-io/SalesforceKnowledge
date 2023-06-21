using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Dtos
{
    public class FileInfoDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string LatestPublishedVersionId { get; set; }
        public string FileExtension { get; set; }
    }
}
