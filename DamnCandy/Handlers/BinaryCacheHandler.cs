using DamnCandy.Operations;

namespace DamnCandy.Handlers;

public class BinaryCacheHandler : ICacheHandler
{
    public string FileExtension { get; }

    public BinaryCacheHandler() => FileExtension = CacheSettings.DefaultBinaryFileExtension;
    
    public BinaryCacheHandler(string fileExtension) => FileExtension = fileExtension;
    
    public async Task SaveBytesAsync(Guid guid, byte[] bytes)
    {
        var path = $"{CacheSettings.CacheDataPath}/{guid}/Data.{FileExtension}";
        await File.WriteAllBytesAsync(path, bytes);
    }

    public async Task<byte[]> GetBytesAsync(Guid guid)
    {
        var path = $"{CacheSettings.CacheDataPath}/{guid}/Data.{FileExtension}";
        return await File.ReadAllBytesAsync(path);
    }

    public async Task<T> LoadAsync<T>(Guid guid)
    {
        if (typeof(T) != typeof(byte[]))
            throw new Exception("Get object from binary cache handler is not supported!");
        
        return (T)(object)await GetBytesAsync(guid);
    }
        
}