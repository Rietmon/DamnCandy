using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using System.Text.Json;
#endif

namespace DamnCandy.Metadatas
{
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
#if UNITY_5_3_OR_NEWER
            metadata = JsonUtility.FromJson<CacheMetadata>(json);
#else
            metadata = JsonSerializer.Deserialize<CacheMetadata>(json);
#endif
            LoadedMetadatas.Add(guid, metadata);
            return metadata;
        }

        public static void Save(this CacheMetadata metadata)
        {
            var path = $"__CacheData/{metadata.Guid}/Metadata.json";
#if UNITY_5_3_OR_NEWER
            var json = JsonUtility.ToJson(metadata);
#else 
            var json = JsonSerializer.Serialize(metadata);
#endif
            File.WriteAllText(path, json);
        }

        internal static void Delete(Guid guid) => LoadedMetadatas.Remove(guid);
    }
}