using DamnCandy.Handlers;
using DamnCandy.Metadatas;
using DamnCandy.Operations;
using DamnCandy.Providers;
using DamnCandy.Providers.Jsons;
using DamnCandy.Utilities;

namespace DamnCandy;

/// <summary>
/// Main manager for DamnCandy. Provides API for caching and loading data.
/// </summary>
public static class CacheManager
{
    /// <summary>
    /// Operations that are currently running.
    /// DO NOT REMOVE OR MODIFY ANYTHING FROM THIS LIST!
    /// READ ONLY!
    /// </summary>
    public static List<CacheOperation> Operations { get; } = new();

    /// <summary>
    /// Begin caching operation and return CacheOperation
    /// </summary>
    /// <param name="provider">Cache provider, where fetch data, creates guid etc</param>
    /// <param name="handler">Cache handler, how to write file, read etc</param>
    /// <param name="cacheId">Additional id. If Guid unable to create until value isn't fetched for Guid will be used this string</param>
    /// <returns>CacheOperation object which controls your task and provide info and management API</returns>
    public static CacheOperation Cache(ICacheProvider provider, ICacheHandler handler, string cacheId = "")
    {
        var operation = new CacheOperation(cacheId, provider, handler);
        operation.OnOperationEnded += () => OnOperationEnded(operation);
        Operations.Add(operation);
        operation.Begin();
        return operation;
    }

    /// <summary>
    /// Load value from cache
    /// </summary>
    /// <param name="guid">Guid of cache</param>
    /// <param name="handler">Cache handler, how to write file, read etc</param>
    /// <typeparam name="T">Target cache value type</typeparam>
    /// <returns>Container with Metadata and target value</returns>
    public static async Task<CacheContainer<T>> LoadAsync<T>(Guid guid, ICacheHandler handler)
    {
        var metadata = CacheMetadatasManager.Load(guid);
        if (metadata == null)
            return null;

        var value = await handler.LoadAsync<T>(guid);
        return new CacheContainer<T>(metadata, value);
    }

    /// <summary>
    /// Load value from cache
    /// </summary>
    /// <param name="cacheId">CacheId of cache</param>
    /// <param name="handler">Cache handler, how to write file, read etc</param>
    /// <typeparam name="T">Target cache value type</typeparam>
    /// <returns>Container with Metadata and target value</returns>
    public static async Task<CacheContainer<T>> LoadAsync<T>(string cacheId, ICacheHandler handler) =>
        await LoadAsync<T>(cacheId.CreateGuid(), handler);

    /// <summary>
    /// Returns true if guid there is in cache
    /// </summary>
    /// <param name="guid">Guid of cache</param>
    /// <returns>True if cached</returns>
    public static bool IsCached(Guid guid) => CacheMetadatasManager.HasMetadata(guid);
    
    /// <summary>
    /// Returns true if guid there is in cache
    /// </summary>
    /// <param name="provider">Cache provider, where fetch data, creates guid etc</param>
    /// <returns>True if cached</returns>
    public static bool IsCached(ICacheProvider provider) => provider.CanProvideGuidBeforeFetch 
        ? IsCached(provider.GetGuid())
        : throw new Exception($"Provider {provider.GetType()} not supports providing guid before fetch!");
    
    /// <summary>
    /// Returns true if guid there is in cache
    /// </summary>
    /// <param name="cacheId">CacheId of cache</param>
    /// <returns>True if cached</returns>
    public static bool IsCached(string cacheId) => IsCached(cacheId.CreateGuid());
    
    /// <summary>
    /// Returns CacheOperation by guid if it's running now
    /// </summary>
    /// <param name="guid">Guid of cache</param>
    /// <returns>CacheOperation</returns>
    public static CacheOperation GetOperation(Guid guid) => Operations.FirstOrDefault((operation) 
        => operation.Guid == guid);
    
    /// <summary>
    /// Returns CacheOperation by guid if it's running now
    /// </summary>
    /// <param name="provider">Cache provider, where fetch data, creates guid etc</param>
    /// <returns>CacheOperation</returns>
    public static CacheOperation GetOperation(ICacheProvider provider) => provider.CanProvideGuidBeforeFetch 
        ? GetOperation(provider.GetGuid())
        : throw new Exception($"Provider {provider.GetType()} not supports providing guid before fetch!");
    
    /// <summary>
    /// Returns CacheOperation by guid if it's running now
    /// </summary>
    /// <param name="cacheId">CacheId of cache</param>
    /// <returns>CacheOperation</returns>
    public static CacheOperation GetOperation(string cacheId) => Operations.FirstOrDefault((operation) 
        => operation.CacheId == cacheId);

    /// <summary>
    /// Returns true if there is running operation with provided argument
    /// </summary>
    /// <param name="guid">Guid of cache</param>
    /// <returns>True if there is operation</returns>
    public static bool IsCaching(Guid guid) => Operations.Exists((operation) => operation.Guid == guid);
    
    /// <summary>
    /// Returns true if there is running operation with provided argument
    /// </summary>
    /// <param name="provider">Cache provider, where fetch data, creates guid etc</param>
    /// <returns>True if there is operation</returns>
    public static bool IsCaching(ICacheProvider provider) => provider.CanProvideGuidBeforeFetch 
        ? IsCaching(provider.GetGuid())
        : throw new Exception($"Provider {provider.GetType()} not supports providing guid before fetch!");
    
    /// <summary>
    /// Returns true if there is running operation with provided argument
    /// </summary>
    /// <param name="cacheId">CacheId of cache</param>
    /// <returns>True if there is operation</returns>
    public static bool IsCaching(string cacheId) => Operations.Exists((operation) => operation.CacheId == cacheId);

    /// <summary>
    /// Delete cache from memory and kill operation if it's running
    /// </summary>
    /// <param name="guid">Guid of cache</param>
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