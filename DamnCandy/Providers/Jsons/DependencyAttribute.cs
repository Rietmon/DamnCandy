using DamnCandy.Handlers;

namespace DamnCandy.Providers.Jsons;

public class DependencyAttribute : Attribute
{
    public Type CacheProvider { get; }
    public Type CacheHandler { get; }
    public DependencyCacheIdFormat CacheIdFormat { get; }
    
    public DependencyAttribute(Type provider, Type handler, DependencyCacheIdFormat cacheIdFormat)
    {
        CacheProvider = provider;
        CacheHandler = handler;
        CacheIdFormat = cacheIdFormat;
    }
}