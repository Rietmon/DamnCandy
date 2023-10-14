using DamnCandy.Providers.Jsons;

namespace DamnCandy.Providers;

public interface ICacheProvider
{
    bool CanProvideGuidBeforeFetch { get; }
    
    bool ProcessDependencies { get; }
    
    Task<byte[]> ProcessCacheAsync();
    
    Guid GetGuid();

    IEnumerable<DependencyData> GetDependencies();
}