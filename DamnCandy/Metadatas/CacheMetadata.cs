using System.Text.Json.Serialization;

namespace DamnCandy.Metadatas;

[Serializable]
public class CacheMetadata
{
    [JsonInclude] public Guid Guid { get; internal set; }
    
    [JsonInclude] public string CacheId { get; internal set; }
    
    [JsonInclude] public DateTime CacheDate { get; internal set; }
    
    [JsonInclude] public bool IsValid { get; internal set; }
    
    [JsonInclude] public Guid[] Dependencies { get; internal set; }
    
    [JsonInclude] public bool IsDependenciesValid { get; internal set; }
    
    public CacheMetadata() { }
}