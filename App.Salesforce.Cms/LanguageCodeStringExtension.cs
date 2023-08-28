namespace App.Salesforce.Cms;

public static class LanguageCodeStringExtension
{
    public static string ToLanguageHeaderFormat(this string str)
    {
        return str.ToLower().Replace("_", "-");
    }
}