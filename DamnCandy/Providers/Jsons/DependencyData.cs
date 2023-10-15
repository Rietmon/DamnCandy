namespace DamnCandy.Providers.Jsons;

public class DependencyData
{
    /// <summary>
    /// Dependency value
    /// </summary>
    public object Value { get; }
    
    /// <summary>
    /// Dependency value type
    /// </summary>
    public Type ValueType { get; }
    
    /// <summary>
    /// Dependency field or property name
    /// </summary>
    public string ValueName { get; }
    
    /// <summary>
    /// Type of cache provider
    /// </summary>
    public Type CacheProviderType { get; }
    
    /// <summary>
    /// Type of cache handler
    /// </summary>
    public Type CacheHandlerType { get; }
    
    /// <summary>
    /// How to create cache id
    /// </summary>
    public DependencyCacheIdFormat CacheIdFormat { get; }
    
    public DependencyData(object value, Type valueType, string valueName, Type cacheProviderType, Type cacheHandlerType, DependencyCacheIdFormat cacheIdFormat)
    {
        Value = value;
        ValueType = valueType;
        ValueName = valueName;
        CacheProviderType = cacheProviderType;
        CacheHandlerType = cacheHandlerType;
        CacheIdFormat = cacheIdFormat;
    }
}