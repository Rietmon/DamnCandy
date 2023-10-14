namespace DamnCandy.Providers.Jsons;

public class DependencyData
{
    public object Value { get; }
    public Type ValueType { get; }
    public string ValueName { get; }
    public Type CacheProviderType { get; }
    public Type CacheHandlerType { get; }
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