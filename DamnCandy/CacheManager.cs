using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DamnCandy.Handlers;
using DamnCandy.Metadatas;
using DamnCandy.Operations;
using DamnCandy.Providers;
using DamnCandy.Providers.Jsons;
using DamnCandy.Utilities;

namespace DamnCandy;

public static class CacheManager
{
    public static List<CacheOperation> Operations { get; } = new();

    public static CacheOperation Cache(ICacheProvider provider, ICacheHandler handler, string cacheId = "")
    {
        var operation = new CacheOperation(cacheId, provider, handler);
        operation.OnOperationEnded += () => OnOperationEnded(operation);
        Operations.Add(operation);
        operation.Begin();
        return operation;
    }

    public static async Task<CacheContainer<T>> LoadAsync<T>(Guid guid, ICacheHandler handler)
    {
        var metadata = CacheMetadatasManager.Load(guid);
        if (metadata == null)
            return null;

        var value = await handler.LoadAsync<T>(guid);
        return new CacheContainer<T>(metadata, value);
    }

    public static async Task<CacheContainer<T>> LoadAsync<T>(string cacheId, ICacheHandler handler) =>
        await LoadAsync<T>(cacheId.CreateGuid(), handler);

    public static bool IsCached(Guid guid) => CacheMetadatasManager.HasMetadata(guid);
    public static bool IsCached(ICacheProvider provider) => provider.CanProvideGuidBeforeFetch 
        ? IsCached(provider.GetGuid())
        : throw new Exception($"Provider {provider.GetType()} not supports providing guid before fetch!");
    public static bool IsCached(string cacheId) => IsCached(cacheId.CreateGuid());
    
    public static CacheOperation GetOperation(Guid guid) => Operations.FirstOrDefault((operation) 
        => operation.Guid == guid);
    public static CacheOperation GetOperation(ICacheProvider provider) => provider.CanProvideGuidBeforeFetch 
        ? GetOperation(provider.GetGuid())
        : throw new Exception($"Provider {provider.GetType()} not supports providing guid before fetch!");
    public static CacheOperation GetOperation(string cacheId) => Operations.FirstOrDefault((operation) 
        => operation.CacheId == cacheId);

    public static bool IsCaching(Guid guid) => Operations.Exists((operation) => operation.Guid == guid);
    public static bool IsCaching(ICacheProvider provider) => provider.CanProvideGuidBeforeFetch 
        ? IsCaching(provider.GetGuid())
        : throw new Exception($"Provider {provider.GetType()} not supports providing guid before fetch!");
    public static bool IsCaching(string cacheId) => Operations.Exists((operation) => operation.CacheId == cacheId);

    public static void Delete(Guid guid)
    {
        if (guid == Guid.Empty)
            return;
        
        var operation = GetOperation(guid);
        operation?.Cancel();
        
        if (!IsCached(guid))
            return;

        var metadata = CacheMetadatasManager.Load(guid);
        FileSystemUtilities.ForceDeleteDirectory($"{CacheSettings.CacheDataPath}/{guid}");
        
        if (metadata.Dependencies != null)
        {
            foreach (var dependencyGuid in metadata.Dependencies)
                FileSystemUtilities.ForceDeleteDirectory($"{CacheSettings.CacheDataPath}/{dependencyGuid}");
        }
        
        CacheMetadatasManager.Delete(guid);
    }
    
    internal static CacheOperation ProcessDependency(DependencyData dependency, CacheOperation parent)
    {
        var provider = (ICacheProvider)Activator.CreateInstance(dependency.CacheProviderType, dependency.Value);
        var handler = (ICacheHandler)Activator.CreateInstance(dependency.CacheHandlerType);
        var operation = new CacheOperation(CreateCacheIdForDependency(dependency, parent), provider, handler, parent);
        operation.OnOperationEnded += () => OnOperationEnded(operation);
        Operations.Add(operation);
        operation.Begin();
        return operation;
    }

    private static string CreateCacheIdForDependency(DependencyData dependency, CacheOperation parent) =>
        dependency.CacheIdFormat switch
        {
            DependencyCacheIdFormat.FromValue => dependency.Value.ToString(),
            DependencyCacheIdFormat.FromParentAndName => $"{parent.CacheId}_{dependency.ValueName}",
            DependencyCacheIdFormat.FromParentGuidAndName => $"{parent.Guid}_{dependency.ValueName}",
            DependencyCacheIdFormat.DontCreate => string.Empty,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static void OnOperationEnded(CacheOperation operation)
    {
        Operations.Remove(operation);

        if (operation.Status is CacheStatus.Failed or CacheStatus.Cancelled)
            Delete(operation.Guid);
    }
}