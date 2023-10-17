using DamnCandy;
using DamnCandy.Example;
using DamnCandy.Handlers.Binaries;
using DamnCandy.Handlers.Jsons;
using DamnCandy.Providers.Jsons;

const string Url = "http://localhost:5043/";

CacheSettings.DefaultBinaryFileExtension = "png";

var provider = new RestJsonModelCacheProvider<TestModel>(Url);
var handler = new JsonCacheHandler();

var operation = CacheManager.Cache(provider, handler);
var operationWithCacheId = CacheManager.Cache(provider, handler, "TestCacheId");

await operation.HandlerTask;
await operationWithCacheId.HandlerTask;

// It's second one instance of TestModel, do not cache twice it! This is just example!
var cacheContainerFromProvider = await CacheManager.LoadAsync<TestModel>(provider, handler);
var cacheContainerFromGuid = await CacheManager.LoadAsync<TestModel>(operation.Guid, handler);

// It's second one instance of TestModel, do not cache twice it! This is just example!
var cacheContainerFromCacheId = await CacheManager.LoadAsync<TestModel>("TestCacheId", handler);

var binaryHandler = new BinaryCacheHandler();
var cacheContainer1UrlBytes = await CacheManager.LoadAsync<byte[]>(cacheContainerFromProvider.Value.TestUrlStringWhichContainsUrl, binaryHandler);
    
Console.ReadKey();