
using System.IO;

namespace NYaul.IO.FileProvider;

public class DefaultFileProvider : IFileProvider
{
    public static readonly DefaultFileProvider Instance = new DefaultFileProvider();

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public Stream OpenRead(string path)
    {
        return File.OpenRead(path);
    }

    public Stream OpenWrite(string path)
    {
        return File.OpenWrite(path).TruncateOnDisposal();
    }
}