using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Salesforce.Cms.Utils
{
    public static class ErrorHandler
    {

        public static async Task<T> ExecuteWithErrorHandling<T>(Func<Task<T>> func)
        {
            try
            {
                return await func.Invoke();
            }
            catch (Exception e)
            {
                throw new PluginApplicationException(e.Message);
            }
        }

        public static async Task ExecuteWithErrorHandling(Func<Task> func)
        {
            try
            {
                await func.Invoke();
            }
            catch (Exception e)
            {
                throw new PluginApplicationException(e.Message);
            }
        }
    }
}
