using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DamnCandy.Providers.Jsons;

namespace DamnCandy.Providers
{
    public interface ICacheProvider
    {
        /// <summary>
        /// If true, Guid will be created before fetching data
        /// </summary>
        bool CanProvideGuidBeforeFetch { get; }
    
        /// <summary>
        /// If true provided model or object has some dependencies
        /// </summary>
        bool ProcessDependencies { get; }
    
        /// <summary>
        /// Process cache data, like fetching data from web, reading file, checking dependencies etc
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ProcessCacheAsync();
    
        /// <summary>
        /// Creates guid for cache
        /// </summary>
        /// <returns></returns>
        Guid GetGuid();

        /// <summary>
        /// If ProcessDependencies is true, this method returns DependencyData array
        /// </summary>
        /// <returns></returns>
        IEnumerable<DependencyData> GetDependencies();
    }
}