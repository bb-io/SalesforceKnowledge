namespace App.Salesforce.Cms;

public static class LanguageCodeStringExtension
{
    public static string ToLanguageHeaderFormat(this string str)
        => str.ToLower().Replace("_", "-");
}