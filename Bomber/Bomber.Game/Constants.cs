using GameFramework.Objects;

namespace Bomber.Game;

public static class Constants
{
    public static void CreateFile(string path)
    {
        if (File.Exists(path)) return;
            
        File.Create(path).Close();
    }

    public static void CreateDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path) ?? "";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
        
    public static void CreateFileAndDirectory(string path)
    {
        CreateDirectory(path);
        CreateFile(path);
    }
}