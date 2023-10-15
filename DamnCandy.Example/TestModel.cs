using DamnCandy.Providers.Jsons;

namespace DamnCandy.Example;

[Serializable]
public class TestModel
{
    public string TestCommonString { get; set; }
    [Dependency()] public string TestUrlStringWhichContainsUrl { get; set; }
}