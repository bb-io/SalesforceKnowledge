using Apps.Salesforce.Cms.Models.Dtos;
using HtmlAgilityPack;
using System.Net;

namespace Apps.Salesforce.Cms.Helper;

public static class HtmlHelper
{
    public static HtmlDocument GenerateHtml(Dictionary<string, CustomContentFieldDto> contentFields)
    {
        var doc = new HtmlDocument();

        var htmlNode = doc.CreateElement("html");
        doc.DocumentNode.AppendChild(htmlNode);

        var headNode = doc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var bodyNode = doc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        foreach (var field in contentFields)
        {
            var containerDiv = doc.CreateElement("div");
            containerDiv.SetAttributeValue("data-fieldName", field.Key);

            var labelHeader = doc.CreateElement("h3");
            labelHeader.InnerHtml = field.Value.Label;

            var contentDiv = doc.CreateElement("div");
            contentDiv.InnerHtml = field.Value.Value;

            containerDiv.AppendChild(labelHeader);
            containerDiv.AppendChild(contentDiv);
            bodyNode.AppendChild(containerDiv);
        }

        return doc;
    }

    public static void InjectHeadMetadata(HtmlDocument doc, string content, string metadataId)
    {
        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head == null) 
            return;

        var existingMetas = doc.DocumentNode.SelectNodes($"//meta[@name='{metadataId}']");
        if (existingMetas != null)
        {
            foreach (var meta in existingMetas)
                meta.Remove();
        }

        var newMeta = doc.CreateElement("meta");
        newMeta.SetAttributeValue("name", metadataId);
        newMeta.SetAttributeValue("content", content);
        head.PrependChild(newMeta);
    }

    public static void InjectTitle(HtmlDocument doc, string title)
    {
        if (string.IsNullOrWhiteSpace(title)) 
            return;

        var head = doc.DocumentNode.SelectSingleNode("//head");
        if (head == null) 
            return;

        var titleNode = head.SelectSingleNode("title");
        if (titleNode == null)
        {
            titleNode = doc.CreateElement("title");
            head.PrependChild(titleNode);
        }

        titleNode.RemoveAllChildren();
        titleNode.AppendChild(doc.CreateTextNode(title));
    }

    public static string? ExtractHeadMetadata(HtmlDocument doc, string metadataId)
    {
        var metaTag = doc.DocumentNode.SelectSingleNode($"//meta[@name='{metadataId}']");
        return metaTag?.GetAttributeValue("content", string.Empty);
    }

    public static string? ExtractTitle(HtmlDocument doc)
    {
        var titleNode = doc.DocumentNode.SelectSingleNode("//head/title");

        if (titleNode == null)
            return null;

        return WebUtility.HtmlDecode(titleNode.InnerText);
    }
}
