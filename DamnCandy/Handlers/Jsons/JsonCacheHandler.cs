using System;
using System.IO;
using System.Threading.Tasks;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using System.Text.Json;
#endif

namespace DamnCandy.Handlers.Jsons
{
    /// <summary>
    /// Json cache handler.
    /// Can async save and load json string from file.
    /// </summary>
    public class JsonCacheHandler : ICacheHandler
    {
        public async Task SaveBytesAsync(Guid guid, byte[] bytes)
        {
            var path = $"{CacheSettings.CacheDataPath}/{guid}/Data.json";
            await File.WriteAllBytesAsync(path, bytes);
        }

        public async Task<byte[]> GetBytesAsync(Guid guid)
        {
            var path = $"{CacheSettings.CacheDataPath}/{guid}/Data.json";
            return await File.ReadAllBytesAsync(path);
        }

        public async Task<T> LoadAsync<T>(Guid guid)
        {
            var path = $"{CacheSettings.CacheDataPath}/{guid}/Data.json";
            var json = await File.ReadAllTextAsync(path);
#if UNITY_5_3_OR_NEWER
            return JsonUtility.FromJson<T>(json);
#else
            return JsonSerializer.Deserialize<T>(json);
#endif
        }
    }
}