using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DamnCandy.Providers.Jsons;
using DamnCandy.Utilities;

namespace DamnCandy.Providers.Binaries
{
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

        public Guid GetGuid() => Url.CreateGuid();

        public IEnumerable<DependencyData> GetDependencies() =>
            throw new Exception("RestBinaryCacheProvider does not support dependencies!");
    }
}