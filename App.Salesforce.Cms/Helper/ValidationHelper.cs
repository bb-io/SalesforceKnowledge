using Apps.Salesforce.Cms.Models.Utility.Filters;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Salesforce.Cms.Helper;

public static class ValidationHelper
{
    public static void ValidateDates(this IDateFilter input)
    {
        List<string> errors = [];

        if (input is ICreatedDateRange c &&
            c.CreatedAfter.HasValue && c.CreatedBefore.HasValue &&
            c.CreatedAfter > c.CreatedBefore)
        {
            errors.Add("'Created after' date cannot be later than 'Created before' date");
        }

        if (input is IUpdatedDateRange u &&
            u.UpdatedAfter.HasValue && u.UpdatedBefore.HasValue &&
            u.UpdatedAfter > u.UpdatedBefore)
        {
            errors.Add("'Updated after' date cannot be later than 'Updated before' date");
        }

        if (input is IPublishedDateRange p &&
            p.PublishedAfter.HasValue && p.PublishedBefore.HasValue &&
            p.PublishedAfter > p.PublishedBefore)
        {
            errors.Add("'Published after' date cannot be later than 'Published before' date");
        }

        if (errors.Count() > 0)
            throw new PluginMisconfigurationException(string.Join(". ", errors));
    }
}
