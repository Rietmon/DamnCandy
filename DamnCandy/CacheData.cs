// using System.Text.Json.Serialization;
//
// namespace DamnCandy;
//
// [Serializable]
// public class CacheData
// {
//     [JsonInclude] private Dictionary<string, Guid> CachedIds { get; set; } = new();
//     
//     public void Add(string cacheId, Guid guid) => CachedIds.Add(cacheId, guid);
//     
//     public void Remove(string cacheId) => CachedIds.Remove(cacheId);
//     
//     public Guid TryGetGuid(string cacheId) => CachedIds.TryGetValue(cacheId, out var guid) ? guid : Guid.Empty;
// }