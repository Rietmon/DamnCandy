namespace DamnCandy;

public static class CacheSettings
{
    public static string RootPath { get; set; } = Environment.CurrentDirectory;
    public static string CacheDataFolder { get; set; } = "__CacheData";
    
    public static string DefaultBinaryFileExtension { get; set; } = "bin";
    
    internal static string CacheDataPath => Path.Combine(RootPath, CacheDataFolder);
}