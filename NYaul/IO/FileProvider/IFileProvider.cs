using System.IO;

namespace NYaul.IO.FileProvider;

public interface IFileProvider
{
    bool FileExists(string path);

    Stream OpenRead(string path);

    Stream OpenWrite(string path);
}

public static class FileProviderExtensions
{
    public static string ReadAllText(this IFileProvider fileProvider, string path)
    {
        using var stream = fileProvider.OpenRead(path);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}