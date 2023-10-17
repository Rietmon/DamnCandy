using System.Text.Json.Serialization;
using DamnCandy.Handlers.Binaries;
using DamnCandy.Providers.Binaries;
using DamnCandy.Providers.Jsons;

namespace DamnCandy.Example;

[Serializable]
public class TestModel
{
    [JsonPropertyName("testCommonString")] public string TestCommonString { get; set; }
    [Dependency(typeof(RestBinaryCacheProvider), typeof(BinaryCacheHandler), DependencyCacheIdFormat.FromValue)] 
    [JsonPropertyName("testUrlStringWhichContainsUrl")] public string TestUrlStringWhichContainsUrl { get; set; }
}