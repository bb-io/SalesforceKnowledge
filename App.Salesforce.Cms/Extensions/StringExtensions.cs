namespace App.Salesforce.Cms.Extensions;

public static class StringExtensions
{
    public static string WithQuery(this string str, Dictionary<string, string>? query)
    {
        if (query is null || !query.Any())
            return str;

        var queryString = string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));
        return str.Contains("?")
            ? $"{str}&{queryString}"
            : $"{str}?{queryString}";
    }
}