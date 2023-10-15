using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DamnCandy.Providers.Jsons;

/// <summary>
/// Rest method to get json data from url.
/// Supports only GET method.
/// Supports dependencies.
/// Creates guid from url.
/// </summary>
public class RestJsonModelCacheProvider<T> : ICacheProvider where T : new()
{
    public bool CanProvideGuidBeforeFetch => true;
    
    public bool ProcessDependencies => dependencyProperties.Length > 0 || dependencyFields.Length > 0;
    
    public string Url { get; }
    
    private T model;
    private PropertyInfo[] dependencyProperties;
    private FieldInfo[] dependencyFields;
    
    public RestJsonModelCacheProvider(string url) => Url = url;

    public async Task<byte[]> ProcessCacheAsync()
    {
        var httpClient = new HttpClient();

        var modelType = typeof(T);
        var attributeType = typeof(DependencyAttribute);

        dependencyProperties = GetPropertyWithAttribute(modelType, attributeType).ToArray();
        dependencyFields = GetFieldWithAttribute(modelType, attributeType).ToArray();

        if (!ProcessDependencies)
            return await httpClient.GetByteArrayAsync(Url);
        
        var json = await httpClient.GetStringAsync(Url);
        model = JsonSerializer.Deserialize<T>(json);
        return Encoding.UTF8.GetBytes(json);
    }

    public Guid GetGuid()
    {
        var bytes = Encoding.UTF8.GetBytes(Url);
        var hash = MD5.HashData(bytes);
        return new Guid(hash);
    }

    public IEnumerable<DependencyData> GetDependencies()
    {
        var dependencies = new List<DependencyData>();
        foreach (var property in dependencyProperties)
        {
            var value = property.GetValue(model);
            if (value == null)
                continue;
            
            var attribute = property.GetCustomAttribute<DependencyAttribute>()!;
            var dependency =
                new DependencyData(value, value.GetType(), property.Name,
                    attribute.CacheProvider, attribute.CacheHandler, attribute.CacheIdFormat);
            dependencies.Add(dependency);
        }
        foreach (var field in dependencyFields)
        {
            var value = field.GetValue(model);
            if (value == null)
                continue;
            
            var attribute = field.GetCustomAttribute<DependencyAttribute>()!;
            var dependency =
                new DependencyData(value, value.GetType(), field.Name,
                    attribute.CacheProvider, attribute.CacheHandler, attribute.CacheIdFormat);
            dependencies.Add(dependency);
        }

        return dependencies.ToArray();
    }

    private static IEnumerable<PropertyInfo> GetPropertyWithAttribute(Type type, Type attribute) =>
        type.GetProperties().Where(prop => Attribute.IsDefined(prop, attribute));

    private static IEnumerable<FieldInfo> GetFieldWithAttribute(Type type, Type attribute) =>
        type.GetFields().Where(field => Attribute.IsDefined(field, attribute));
}