namespace DamnCandy.Utilities;

internal static class FileSystemUtilities
{
    public static void ValidatePath(string path)
    {
        if (Path.HasExtension(path))
            path = Path.GetDirectoryName(path);
            
        void CheckDirectory(string p)
        {
            if (Directory.Exists(p)) return;

            CheckDirectory(Directory.GetDirectoryRoot(p));
            Directory.CreateDirectory(p);
        }

        CheckDirectory(path);
    }
    
    public static void ForceDeleteDirectory(string target)
    {
        var files = Directory.GetFiles(target);
        var directories = Directory.GetDirectories(target);

        foreach (var file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var directory in directories)
        {
            ForceDeleteDirectory(directory);
        }

        Directory.Delete(target, false);
    }
}