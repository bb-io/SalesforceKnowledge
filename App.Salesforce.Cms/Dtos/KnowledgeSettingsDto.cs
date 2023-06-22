using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms.Dtos
{
    public class KnowledgeSettingsDto
    {
        public string DefaultLanguage { get; set; }
        public bool KnowledgeEnabled { get; set; }
        public List<Language> Languages { get; set; }
    }

    public class Language
    {
        public bool Active { get; set; }
        public string Name { get; set; }
    }
}
