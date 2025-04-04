using App.Salesforce.Cms.Auth.OAuth2;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace App.Salesforce.Cms;

public class SalesforceApplication : BaseInvocable, IApplication, ICategoryProvider
{
    private readonly Dictionary<Type, object> _container;

    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Cms, ApplicationCategory.CustomerSupport];
        set { }
    }
    
    public string Name
    {
        get => "Salesforce";
        set { }
    }

    public SalesforceApplication(InvocationContext invocationContext) : base(invocationContext)
    {
        _container = LoadTypes();
    }

    public T GetInstance<T>()
    {
        if (!_container.TryGetValue(typeof(T), out var value))
        {
            throw new InvalidOperationException($"Instance of type '{typeof(T)}' not found");
        }

        return (T)value;
    }

    private Dictionary<Type, object> LoadTypes()
    {
        return new Dictionary<Type, object>()
        {
            { typeof(IOAuth2AuthorizeService), new OAuth2AuthorizeService(InvocationContext) },
            { typeof(IOAuth2TokenService), new OAuth2TokenService(InvocationContext) }
        };
    }
}