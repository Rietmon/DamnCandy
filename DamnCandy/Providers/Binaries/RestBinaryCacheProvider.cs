using System.Security.Cryptography;
using System.Text;
using DamnCandy.Providers.Jsons;

namespace DamnCandy.Providers.Binaries;

/// <summary>
/// Rest method to get binary data from url.
/// Supports only GET method.
/// Don't supports dependencies.
/// Creates guid from url.
/// </summary>
public class RestBinaryCacheProvider : ICacheProvider
{
    public bool CanProvideGuidBeforeFetch => true;
    
    public bool ProcessDependencies => false;
    
    public string Url { get; }
    
    public RestBinaryCacheProvider(string url) => Url = url;

    public async Task<byte[]> ProcessCacheAsync()
    {
        var httpClient = new HttpClient();
        return await httpClient.GetByteArrayAsync(Url);
    }

    public Guid GetGuid()
    {
        var bytes = Encoding.UTF8.GetBytes(Url);
        var hash = MD5.HashData(bytes);
        return new Guid(hash);
    }

    public IEnumerable<DependencyData> GetDependencies() =>
        throw new Exception("RestBinaryCacheProvider does not support dependencies!");
}