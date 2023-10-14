using DamnCandy.Metadatas;

namespace DamnCandy;

public class CacheContainer<T>
{
    public CacheMetadata Metadata { get; }
    public T Value { get; }

    public CacheContainer(CacheMetadata metadata, T value)
    {
        Metadata = metadata;
        Value = value;
    }
}