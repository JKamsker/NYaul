using System;
using System.IO;
using System.Text;

using NYaul.Internals;

//using Random = NYaul.Internals.RandomEx;

namespace NYaul.IO;

public class TempDirectory : IDisposable
{
    public string Path { get; }

    public bool Exists => Directory.Exists(Path);

    public TempDirectory(string path)
    {
        Path = path;
    }

    // alias CreateTempDirectory
    public static TempDirectory CreateTempDirectory()
        => CreateRandom();

    public static TempDirectory CreateRandom()
    {
        var tempDirectory = System.IO.Path.GetTempPath();
        while (true)
        {
            var randomName = RandomPolyfill.Shared.NextString(10);
            var sb = new StringBuilder();
            sb.Append(tempDirectory);
            sb.Append(randomName);

            var newPath = sb.ToString();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
                return new TempDirectory(newPath);
            }
        }
    }

    public void Dispose()
    {
        Directory.Delete(Path, true);
    }

    public FileInfo GetFile(string name)
    {
        var path = System.IO.Path.Combine(Path, name);
        return new FileInfo(path);
    }

    public DirectoryInfo GetDirectory(string name)
    {
        var path = System.IO.Path.Combine(Path, name);
        return new DirectoryInfo(path);
    }

    public FileInfo CreateFileWithExtension(string extension)
    {
        var path = System.IO.Path.Combine(Path, RandomPolyfill.Shared.NextString(10) + extension);
        File.Create(path).Dispose();
        return new FileInfo(path);
    }

    // CreateDirectory
    public DirectoryInfo CreateSubDirectory(string name)
    {
        var path = System.IO.Path.Combine(Path, name);
        return Directory.CreateDirectory(path);
    }
}