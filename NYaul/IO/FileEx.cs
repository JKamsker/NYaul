using System.IO;

//using Random = NYaul.Internals.RandomEx;

namespace NYaul.IO;

public static class FileEx
{
    public static void DeleteIfExists(string filename)
    {
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
    }

    public static FileStream OpenReadAsync(string filename)
    {
        return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
    }
}