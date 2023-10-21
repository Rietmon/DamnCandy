# DamnCandy
Caching system for .NET/Unity. Supports Binary/JSON serialization, dependencies and full asynchronous.

## How to install
There are two options: for .NET7 projects (didn't test for .NET6 but I think it should works properly) and for Unity projects.

### .NET7
- Download release package for .NET7.
- Reference assembly from package.

### Unity
- Download release package for Unity.
- Import scripts from package to your project.
- (Recommended) Create Assembly Reference for the project.

## How to use
Before use check exmple project. It has basic functionallity.  

### Stages
Caching is splitted to some parts (depends on format, is there dependencies etc.):
1. Not started
2. Downloading - fetching bytes or string from URL
3. Creating Guid - creating guid. By default it takes as base provided URL, hash it with MD5 and creates Guid from bytes. This is unique id for each cached item. You can always chech is cached Guid, get metadata with DateTime of caching, dependencies, or load cached value
4. Creating Metadata - creating metadata for the item. It contains DateTime, guids of dependencies, CacheId if it's provided etc.
5. Saving bytes - saving to storage bytes of cached value (or JSON)
6. Resolving Dependencies - if model contains dependency like URLs to images it also will be cached as separeted value. For dependencies you NEED to provide method how could be created CacheId (by default it's depends on the value)
7. Ended - operation ended. You can check is ended with errors or not.

### Types for caching
If you going to cache any media format, such as Image, Media you can use Binary method. It's just fetch bytes form URL and saving it to the storage. Out-of-the-box supports only byte[] to load.  
If you want to cache model I recommend to use JSON serialization method. It supports to cache dependencies. Out-of-the-box supports any `Serializable` type to load.  
For .NET7 uses `System.Text.Json`, for Unity uses `JsonUtility`! Keep in mind some restrictions of these methods. DamnCandy is open-source you can easily switch methods to any other!

### How to cache
For the begin you need to chose provider of caching and handler.
Provider fetching data from URL and creating Guid (some models can contains Guid inside itself, so better to delegate this work to provider, by default it creates Guid from URL).
Handler saves value to the storage and load from it.

#### Simple cache PNG image
```c#
// Rest Binary Provider which will uses this url to fetch data
var provider = new RestBinaryCacheProvider("url.com/image.png");
// Binary Handler which will just save bytes to the file with provided extension. Extension without dot (".")
var handler = new BinaryCacheHandler("png");
// Begin caching operation
var operation = CacheManager.Cache(provider, handler);
// [Optionally] Wait for handler task to complete
await operation.HandlerTask;
```

#### Simple cache model
Model for the caching (.NET7 way)
```c#
[Serializable]
public class TestModel
{
    [JsonPropertyName("testCommonString")] public string TestCommonString { get; set; }
}
```
Let's fetch this model and cache it
```c#
// Rest Json Provider which will uses this url to fetch data
var provider = new RestJsonModelCacheProvider<TestModel>("url.com/api/model");
// Json Handler which will just save Json string to the file "Model.json"
var handler = new JsonCacheHandler();
// Begin caching operation
var operation = CacheManager.Cache(provider, handler);
// [Optionally] Wait for handler task to complete
await operation.HandlerTask;
```
As you see this way has a problem with Guid creation - this URL don't specificated for retuns only one model. By logic it can return any models, not only one which we cached.  
In this case you can create your own CacheProvider which will create Guid not from URL but from some internal value inside of model. Or use another feature of DamnCandy - CacheId.  
It allows to manage Guid. You can provide any string which will be used for Guid creation and managing Guids will be delegated to you.  
New one CacheProvider which creates Guid from model internal id
```c#
public class YourModelProvider : ICacheProvider
{
    public Guid GetGuid() => model.Id.ToString().CreateGuid();
}
```
Using CacheId for managing Guids
```c#
var operation = CacheManager.Cache(provider, handler, $"TestModel_{model.Id}");
```
CacheId will be used instead of `GetGuid` from provider but only if it's provider. If CacheId is `string.Empty` ("") it will be ignored. You no need to choose only one way, you can combine them.  
That's all depends on your architecture and backend.

#### Cache model with Image dependency
Imagine you have model which can preview image which provided as URL to image. In this case your struct going to be like
```c#
[Serializable]
public class TestModel
{
    [JsonPropertyName("testCommonString")] public string TestCommonString { get; set; }
    [Dependency(typeof(RestBinaryCacheProvider), typeof(BinaryCacheHandler), DependencyCacheIdFormat.FromValue)] 
    [JsonPropertyName("testUrlStringWhichContainsUrl")] public string TestUrlStringWhichContainsUrl { get; set; }
}
```
You need to add attribute `Dependency` to your field which is contains an URL to some additional content.  
For auto-caching dependencies you also need to provide type of Provider and Handler. In my case dependency is Image, so I going to use Binary for both types.  
Third parameter is SO IMPORTANT! That's a way how you will loading this value. You can select how will be created CacheId for this item, there are 4 options:
- From value (default) (example: `"url.com/image.png"`)
- From parent CacheId + name of field/property (example: `"TestModel_0_TestUrlStringWhichContainsUrl"`)
- From parent Guid + name of field/property (example: `"9420F98C-9194-4A8D-A21D-A5F417E4BEE3_TestUrlStringWhichContainsUrl"`)
- Dont create (will be created by internal Provider method `GetGuid`
I recommend to use "From value" be cause if some models referenced to single one image, image will be cached only once not for each one model separeted!

### Metadata
Metadata contains some data of cached value:
- Guid - guid of cached item
- CacheId - if it was provided
- CacheDate - date and time when item was cached
- IsValid - is cached sucessfuly, don't unexcpected canceled (shutdowned and app while caching)
- Dependencies - array with Guids of dependencies (if they are)
- IsDependenciesValid - same with `IsValid` but for dependencies
