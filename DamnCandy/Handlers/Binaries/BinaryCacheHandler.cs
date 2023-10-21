using System;
using System.IO;
using System.Threading.Tasks;

namespace DamnCandy.Handlers.Binaries
{
    /// <summary>
    /// Binary cache handler.
    /// Can async save and load bytes from file.
    /// Supports only byte[] type to load.
    /// </summary>
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

        public async Task<T> LoadAsync<T>(Guid guid)
        {
            if (typeof(T) != typeof(byte[]))
                throw new Exception($"Provider {GetType()} not supports providing guid before fetch!");
        
            var path = $"{CacheSettings.CacheDataPath}/{guid}/Data.{FileExtension}";
            return (T)(object)await File.ReadAllBytesAsync(path);
        }
        
    }
}