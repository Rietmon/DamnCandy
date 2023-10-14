using System.Text.Json;

namespace DamnCandy.Handlers;

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
        return JsonSerializer.Deserialize<T>(json);
    }
}