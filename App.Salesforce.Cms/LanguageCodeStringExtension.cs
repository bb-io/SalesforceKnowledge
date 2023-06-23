using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Salesforce.Cms
{
    public static class LanguageCodeStringExtension
    {
        public static string ToLanguageHeaderFormat(this string str)
        {
            return str.ToLower().Replace("_", "-");
        }
    }
}
