using System.Text.Json.Serialization;

namespace DamnCandy.Metadatas;

[Serializable]
public class CacheMetadata
{
    /// <summary>
    /// Guid of cache
    /// </summary>
    [JsonInclude] public Guid Guid { get; internal set; }
    
    /// <summary>
    /// CacheId of cache
    /// </summary>
    [JsonInclude] public string CacheId { get; internal set; }
    
    /// <summary>
    /// When cache was created
    /// </summary>
    [JsonInclude] public DateTime CacheDate { get; internal set; }
    
    /// <summary>
    /// Is cache valid and successfully downloaded
    /// </summary>
    [JsonInclude] public bool IsValid { get; internal set; }
    
    /// <summary>
    /// Does cache has dependencies
    /// </summary>
    [JsonInclude] public Guid[] Dependencies { get; internal set; }
    
    /// <summary>
    /// Are dependencies valid
    /// </summary>
    [JsonInclude] public bool IsDependenciesValid { get; internal set; }
    
    public CacheMetadata() { }
}