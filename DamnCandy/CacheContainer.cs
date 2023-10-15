using DamnCandy.Metadatas;

namespace DamnCandy;

/// <summary>
/// Container for cache data
/// </summary>
/// <typeparam name="T">Target cache value type</typeparam>
public class CacheContainer<T>
{
    /// <summary>
    /// Metadata of cache
    /// </summary>
    public CacheMetadata Metadata { get; }
    
    /// <summary>
    /// Target value
    /// </summary>
    public T Value { get; }

    internal CacheContainer(CacheMetadata metadata, T value)
    {
        Metadata = metadata;
        Value = value;
    }
}