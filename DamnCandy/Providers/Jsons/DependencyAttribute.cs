namespace DamnCandy.Providers.Jsons;

/// <summary>
/// Attribute for mark that field contains dependency. By default supports URLs such as images, videos, audio, etc.
/// </summary>
public class DependencyAttribute : Attribute
{
    /// <summary>
    /// Type of Cache provider, where fetch data, creates guid etc
    /// </summary>
    public Type CacheProvider { get; }
    
    /// <summary>
    /// Type of Cache handler, how to write file, read etc
    /// </summary>
    public Type CacheHandler { get; }
    
    /// <summary>
    /// How to create Guid for dependency. Byt default uses internal method from provider which generate Guid by url
    /// </summary>
    public DependencyCacheIdFormat CacheIdFormat { get; }
    
    /// <summary>
    /// Create new instance of DependencyAttribute
    /// </summary>
    /// <param name="provider">Type of Cache provider, where fetch data, creates guid etc</param>
    /// <param name="handler">Type of Cache handler, how to write file, read etc</param>
    /// <param name="cacheIdFormat">How to create Guid for dependency. Byt default uses internal method from provider which generate Guid by url</param>
    public DependencyAttribute(Type provider, Type handler, DependencyCacheIdFormat cacheIdFormat)
    {
        CacheProvider = provider;
        CacheHandler = handler;
        CacheIdFormat = cacheIdFormat;
    }
}