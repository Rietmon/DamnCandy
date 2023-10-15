using System.Text.Json;

namespace DamnCandy.Metadatas;

internal static class CacheMetadatasManager
{
    private static Dictionary<Guid, CacheMetadata> LoadedMetadatas { get; } = new();

    public static bool HasMetadata(Guid guid)
    {
        if (LoadedMetadatas.ContainsKey(guid))
            return true;
        
        var path = $"{CacheSettings.CacheDataPath}/{guid}/Metadata.json";
        return File.Exists(path);
    }
    
    public static CacheMetadata Load(Guid guid)
    {
        if (LoadedMetadatas.TryGetValue(guid, out var metadata))
            return metadata;
        
        var path = $"{CacheSettings.CacheDataPath}/{guid}/Metadata.json";
        if (!File.Exists(path))
            return null;
        
        var json = File.ReadAllText(path);
        metadata = JsonSerializer.Deserialize<CacheMetadata>(json);
        LoadedMetadatas.Add(guid, metadata);
        return metadata;
    }

    public static void Save(this CacheMetadata metadata)
    {
        var path = $"__CacheData/{metadata.Guid}/Metadata.json";
        var json = JsonSerializer.Serialize(metadata);
        File.WriteAllText(path, json);
    }

    internal static void Delete(Guid guid) => LoadedMetadatas.Remove(guid);
}